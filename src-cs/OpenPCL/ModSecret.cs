using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCL.Our;
using PCLCS;

namespace PCL
{
    [StandardModule]
    internal static class ModSecret
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModSecret()
        {
        }

        internal static string SecretCdnSign(string UrlWithMark)
        {
            string text;
            if (ModBase.ApplicationStartTick < 1L)
            {
                text = UrlWithMark;
            }
            else if (!UrlWithMark.EndsWithF("{CDN}", false))
            {
                text = UrlWithMark;
            }
            else
            {
                string text2 = UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20");
                string text3 = ModBase.RandomInteger(0, 2147483645).ToString("x").EnsureLength('0', 8);
                string text4 = Conversions.ToString(ModBase.GetUnixTimestampUtc());
                string text5 = text2.Substring(checked(text2.IndexOfF("://", false) + 3));
                text5 = text5.Substring(text5.IndexOfF("/", false));
                string text6 = CryptographyUtils.ComputeHash(new string[] { text5, text4, text3, "0", "PCL2Server" }.Join("-"), CryptographyUtils.HashMethod.Md5);
                text = text2 + (text2.Contains("?") ? "&" : "?") + "sign=" + new string[] { text4, text3, "0", text6 }.Join("-");
            }
            return text;
        }

        internal static void SecretHeadersSign(string Url, ref HttpRequestMessage Req, bool SimulateBrowserHeaders = false)
        {
            if (ModBase.ApplicationStartTick >= 1L)
            {
                if (!Req.Headers.UserAgent.Any<ProductInfoHeaderValue>())
                {
                    if (Url.Contains("baidupcs.com") || Url.Contains("baidu.com"))
                    {
                        Req.Headers.Add("User-Agent", "LogStatistic");
                    }
                    else if (SimulateBrowserHeaders)
                    {
                        Req.Headers.Add("User-Agent", string.Format("PCL2/{0}.{1} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36", "2.99.0.0", 0));
                    }
                    else
                    {
                        Req.Headers.Add("User-Agent", string.Format("PCL2/{0}.{1}", "2.99.0.0", 0));
                    }
                }
                if (!SimulateBrowserHeaders)
                {
                    Req.Headers.Referrer = new Uri(string.Format("http://{0}.pcl2.server/", 414));
                }
                if (Url.Contains("api.curseforge.com") || Url.Contains("forgecdn.net"))
                {
                    Req.Headers.Add("x-api-key", Encoding.Default.GetString(Convert.FromBase64String("JDJhJDEwJDdXV1YxV0VMY3dYOFhwN2Q2OE1Icy41Z3JUeVpWYTZTeW55ZVN5TWgxcnNFODluSWEwOXpP")));
                }
            }
        }

        public static void ThemeRefresh(int NewTheme = -1)
        {
            try
            {
                if (ModSecret.ThemeNow != NewTheme || NewTheme < 0)
                {
                    if (ModSecret.ThemeCheckOne((NewTheme >= 0) ? NewTheme : ModSecret.ThemeNow))
                    {
                        if (NewTheme >= 0)
                        {
                            ModSecret.ThemeNow = NewTheme;
                        }
                        int themeNow = ModSecret.ThemeNow;
                        checked
                        {
                            switch (themeNow)
                            {
                            case 0:
                                ModSecret.ColorHue = 210;
                                ModSecret.ColorSat = 85;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 1:
                                ModSecret.ColorHue = 180;
                                ModSecret.ColorSat = 50;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 2:
                                ModSecret.ColorHue = 130;
                                ModSecret.ColorSat = 40;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = new int[] { 5, -5, 5 };
                                break;
                            case 3:
                                ModSecret.ColorHue = 75;
                                ModSecret.ColorSat = 50;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 4:
                                ModSecret.ColorHue = 35;
                                ModSecret.ColorSat = 30;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 5:
                                ModSecret.ColorHue = 225;
                                ModSecret.ColorSat = 10;
                                ModSecret.ColorLightAdjust = -20;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 6:
                                ModSecret.ColorHue = 345;
                                ModSecret.ColorSat = 70;
                                ModSecret.ColorLightAdjust = 15;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 7:
                                ModSecret.ColorHue = 285;
                                ModSecret.ColorSat = 50;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 8:
                                ModSecret.ColorHue = 50;
                                ModSecret.ColorSat = 65;
                                ModSecret.ColorLightAdjust = 10;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 9:
                                ModSecret.ColorHue = 25;
                                ModSecret.ColorSat = 75;
                                ModSecret.ColorLightAdjust = 5;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 10:
                                ModSecret.ColorHue = 5;
                                ModSecret.ColorSat = 70;
                                ModSecret.ColorLightAdjust = -5;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 11:
                                ModSecret.ColorHue = 230;
                                ModSecret.ColorSat = 60;
                                ModSecret.ColorLightAdjust = -5;
                                ModSecret.ColorHueTopbarDelta = 0;
                                break;
                            case 12:
                                if (!ModSecret.IsThemeColorfulLoaded)
                                {
                                    ModSecret.IsThemeColorfulLoaded = true;
                                    ModSecret.ColorHue = ModBase.RandomInteger(0, 359);
                                    ModSecret.ColorHueDelta = (ModBase.RandomInteger(0, 1) * 2 - 1) * 4;
                                }
                                ModSecret.ColorHue = (ModSecret.ColorHue + ModSecret.ColorHueDelta) % 360;
                                ModSecret.ColorSat = 45;
                                ModSecret.ColorLightAdjust = 0;
                                break;
                            case 13:
                                ModSecret.ColorHue = 290;
                                ModSecret.ColorSat = 50;
                                ModSecret.ColorLightAdjust = 0;
                                ModSecret.ColorHueTopbarDelta = new int[] { -30, 10, 50 };
                                break;
                            case 14:
                                ModSecret.ColorHue = Settings.Get<int>("UiLauncherHue", null);
                                ModSecret.ColorSat = Settings.Get<int>("UiLauncherSat", null);
                                ModSecret.ColorLightAdjust = Settings.Get<int>("UiLauncherLight", null) - 20;
                                ModSecret.ColorHueTopbarDelta = Settings.Get<int>("UiLauncherDelta", null) - 90;
                                break;
                            default:
                                if (themeNow != 42)
                                {
                                    ModSecret.ColorHue = 210;
                                    ModSecret.ColorSat = 85;
                                    ModSecret.ColorLightAdjust = 0;
                                }
                                else
                                {
                                    ModSecret.ColorHue = 225;
                                    ModSecret.ColorSat = 60;
                                    ModSecret.ColorLightAdjust = -15;
                                    ModSecret.ColorHueTopbarDelta = 0;
                                }
                                break;
                            }
                            switch (ModSecret.ThemeDontClick)
                            {
                            case 1:
                                ModSecret.ColorLightAdjust = 999;
                                break;
                            case 2:
                                ModSecret.ColorHue = ModBase.RandomInteger(0, 359);
                                ModSecret.ColorSat = ModBase.RandomInteger(40, 70);
                                ModSecret.ColorLightAdjust = ModBase.RandomInteger(-20, 20);
                                break;
                            }
                            ModSecret.Color1 = unchecked(new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat * 0.2, 25.0 + (double)ModSecret.ColorLightAdjust * 0.3));
                            ModSecret.Color2 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, (double)(45 + ModSecret.ColorLightAdjust));
                            ModSecret.Color3 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, (double)(55 + ModSecret.ColorLightAdjust));
                            ModSecret.Color4 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, (double)(65 + ModSecret.ColorLightAdjust));
                        }
                        ModSecret.Color5 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 80.0 + (double)ModSecret.ColorLightAdjust * 0.4);
                        ModSecret.Color6 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 91.0 + (double)ModSecret.ColorLightAdjust * 0.1);
                        ModSecret.Color7 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 95.0);
                        ModSecret.Color8 = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 97.0);
                        ModSecret.ColorBg0 = ModSecret.Color4 * 0.4 + ModSecret.Color5 * 0.4 + ModSecret.ColorGray4 * 0.2;
                        ModSecret.ColorBg1 = new ModBase.OurColor(190.0, ModSecret.Color7);
                        ModSecret.ColorSemiTransparent = new ModBase.OurColor(1.0, ModSecret.Color8);
                        Application.Current.Resources["ColorBrush1"] = new SolidColorBrush(ModSecret.Color1);
                        Application.Current.Resources["ColorBrush2"] = new SolidColorBrush(ModSecret.Color2);
                        Application.Current.Resources["ColorBrush3"] = new SolidColorBrush(ModSecret.Color3);
                        Application.Current.Resources["ColorBrush4"] = new SolidColorBrush(ModSecret.Color4);
                        Application.Current.Resources["ColorBrush5"] = new SolidColorBrush(ModSecret.Color5);
                        Application.Current.Resources["ColorBrush6"] = new SolidColorBrush(ModSecret.Color6);
                        Application.Current.Resources["ColorBrush7"] = new SolidColorBrush(ModSecret.Color7);
                        Application.Current.Resources["ColorBrush8"] = new SolidColorBrush(ModSecret.Color8);
                        Application.Current.Resources["ColorBrushBg0"] = new SolidColorBrush(ModSecret.ColorBg0);
                        Application.Current.Resources["ColorBrushBg1"] = new SolidColorBrush(ModSecret.ColorBg1);
                        Application.Current.Resources["ColorObject1"] = (global::System.Windows.Media.Color)ModSecret.Color1;
                        Application.Current.Resources["ColorObject2"] = (global::System.Windows.Media.Color)ModSecret.Color2;
                        Application.Current.Resources["ColorObject3"] = (global::System.Windows.Media.Color)ModSecret.Color3;
                        Application.Current.Resources["ColorObject4"] = (global::System.Windows.Media.Color)ModSecret.Color4;
                        Application.Current.Resources["ColorObject5"] = (global::System.Windows.Media.Color)ModSecret.Color5;
                        Application.Current.Resources["ColorObject6"] = (global::System.Windows.Media.Color)ModSecret.Color6;
                        Application.Current.Resources["ColorObject7"] = (global::System.Windows.Media.Color)ModSecret.Color7;
                        Application.Current.Resources["ColorObject8"] = (global::System.Windows.Media.Color)ModSecret.Color8;
                        Application.Current.Resources["ColorObjectBg0"] = (global::System.Windows.Media.Color)ModSecret.ColorBg0;
                        Application.Current.Resources["ColorObjectBg1"] = (global::System.Windows.Media.Color)ModSecret.ColorBg1;
                        ModSecret.ThemeRefreshMain();
                        if (ModSecret.ThemeNow != 12 && ModSecret.ThemeNow != 14 && ModSecret.ThemeDontClick != 2)
                        {
                            Logger.Info(string.Format("刷新主题：{0}", ModSecret.ThemeNow), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "刷新主题颜色失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
            }
        }

        public static void ThemeRefreshMain()
        {
            ModBase.RunInUi((ModSecret._Closure___._I36_0 == null) ? (ModSecret._Closure___._I36_0 = new Action(ModSecret._Closure___._I._Lambda___36_0)) : ModSecret._Closure___._I36_0, false);
        }

        internal static void ThemeCheckAll(bool EffectSetup)
        {
            ModSecret._Closure___37_0 CS_locals1 = new ModSecret._Closure___37_0(null);
            CS_locals1._VB_Local_EffectSetup = EffectSetup;
            ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
        }

        internal static bool ThemeCheckOne(int Id)
        {
            return (Id >= 0 && Id <= 14) || Id == 42;
        }

        internal static bool ThemeUnlock(int Id, bool ShowDoubleHint = true, string UnlockHint = null)
        {
            // 所有主题默认可用，不再创建解锁记录或重复提示。
            return false;
        }

        internal static void UpdateCheckByButton()
        {
            ModMain.Hint("此构建不提供启动器更新检查，请手动替换程序。", ModMain.HintType.Blue, true);
        }



        public static void UpdateRestart(bool TriggerRestartAndByEnd)
        {
            try
            {
                ModSecret.IsUpdateWaitingRestart = false;
                string text = Paths.Base + "PCL\\OpenPCL.exe";
                string text2 = string.Format("--update {0} \"{1}\" \"{2}\" {3}", new object[]
                {
                    Process.GetCurrentProcess().Id,
                    AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                    AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                    TriggerRestartAndByEnd
                });
                Logger.Info(string.Format("更新程序启动，参数：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                ModBase.StartProcess(new ProcessStartInfo(text)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    Arguments = text2
                });
                if (TriggerRestartAndByEnd)
                {
                    ModMain.FrmMain.EndProgram(false);
                    Logger.Info("已由于更新强制结束程序", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                }
            }
            catch (Win32Exception ex)
            {
                Logger.Warn(ex, "自动更新时触发 Win32 错误，疑似被拦截", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                if (ModMain.OurMsgBox(string.Format("由于被 Windows 安全中心拦截，或者存在权限问题，导致 PCL 无法更新。{0}请将 PCL 所在文件夹加入白名单，或者手动用 {1}PCL\\OpenPCL.exe 替换当前文件！", "\r\n", Paths.Base), "更新失败", "查看帮助", "确定", "", true, true, false, null, null, null) == 1)
                {
                    CustomEvent.Raise(CustomEvent.EventType.打开帮助, "启动器/Microsoft Defender 添加排除项.json");
                }
            }
        }

        public static void UpdateReplace(int ProcessId, string OldFileName, string NewFileName, bool TriggerRestart)
        {
            ModSecret._Closure___46_0 CS_locals1 = new ModSecret._Closure___46_0(null);
            try
            {
                Process.GetProcessById(ProcessId).Kill();
            }
            catch (Exception ex)
            {
            }
            string text = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(OldFileName));
            CS_locals1._VB_Local_NewFilePath = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(NewFileName));
            Exception ex2 = null;
            int num = 0;
            checked
            {
                do
                {
                    try
                    {
                        FileUtils.Delete(text, false);
                        FileUtils.Delete(CS_locals1._VB_Local_NewFilePath, false);
                        if (!FileUtils.Exists(text) && !FileUtils.Exists(CS_locals1._VB_Local_NewFilePath))
                        {
                            break;
                        }
                    }
                    catch (Exception ex3)
                    {
                        ex2 = ex3;
                    }
                    Thread.Sleep(num * 500 + 500);
                    num++;
                }
                while (num <= 4);
                VB_AnonymousDelegate_11<string, string> vb_AnonymousDelegate_ = new VB_AnonymousDelegate_11<string, string>(CS_locals1._Lambda___0);
                if (!FileUtils.Exists(text) && !FileUtils.Exists(CS_locals1._VB_Local_NewFilePath))
                {
                    try
                    {
                        FileUtils.Copy(ModBase.PathExe, CS_locals1._VB_Local_NewFilePath);
                    }
                    catch (UnauthorizedAccessException ex4)
                    {
                        Interaction.MsgBox(string.Format("{0}{1}{2}详细错误信息：{3}{4}", new object[]
                        {
                            vb_AnonymousDelegate_("复制新版本文件时权限不足"),
                            "\r\n",
                            "\r\n",
                            "\r\n",
                            ex4.GetDisplay(true)
                        }), MsgBoxStyle.Critical, "更新失败");
                    }
                    catch (Exception ex5)
                    {
                        Interaction.MsgBox(string.Format("{0}{1}{2}详细错误信息：{3}{4}", new object[]
                        {
                            vb_AnonymousDelegate_("复制新版本文件时出错"),
                            "\r\n",
                            "\r\n",
                            "\r\n",
                            ex5.GetDisplay(true)
                        }), MsgBoxStyle.Critical, "更新失败");
                        return;
                    }
                    if (TriggerRestart)
                    {
                        try
                        {
                            ModBase.StartProcess(CS_locals1._VB_Local_NewFilePath, "");
                        }
                        catch (Exception ex6)
                        {
                            Interaction.MsgBox(string.Format("PCL 更新已完成，但在自动重启 PCL 时遇到了意外，请手动启动 PCL。{0}{1}详细错误信息：{2}{3}", new object[]
                            {
                                "\r\n",
                                "\r\n",
                                "\r\n",
                                ex6.GetDisplay(true)
                            }), MsgBoxStyle.Information, "更新完成");
                        }
                    }
                    return;
                }
                if (ex2 is UnauthorizedAccessException)
                {
                    Interaction.MsgBox(string.Format("{0}{1}{2}详细错误信息：{3}{4}", new object[]
                    {
                        vb_AnonymousDelegate_("删除老版本文件时权限不足"),
                        "\r\n",
                        "\r\n",
                        "\r\n",
                        ex2.GetDisplay(true)
                    }), MsgBoxStyle.Critical, "更新失败");
                    return;
                }
                Interaction.MsgBox(string.Format("{0}{1}{2}详细错误信息：{3}{4}", new object[]
                {
                    vb_AnonymousDelegate_("删除老版本文件时出错"),
                    "\r\n",
                    "\r\n",
                    "\r\n",
                    ex2.GetDisplay(true)
                }), MsgBoxStyle.Critical, "更新失败");
            }
        }



        private static void ServerSub(ModLoader.LoaderTask<int, int> Loader)
        {
            string path = ModBase.PathTemp + "Cache\\ServerConfig.json";
            ModSecret.ServerConfig = null;
            try
            {
                if (FileUtils.Exists(path))
                {
                    ModSecret.ServerConfig = (JObject)FileUtils.ReadAsJson(path, null, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "读取联机配置缓存失败", LogBehavior.ToastIfDebug);
            }
            try
            {
                ModSecret.ServerConfig = (JObject)ModNet.NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/ServerConfig.json{CDN}", null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
                FileUtils.Write(path, ModSecret.ServerConfig.ToString(), null);
            }
            catch (Exception ex)
            {
                if (ModSecret.ServerConfig == null)
                {
                    throw;
                }
                Logger.Warn(ex, "获取联机配置失败，使用本地配置", LogBehavior.ToastIfDebug);
            }
        }



        public static DonationRank CurrentRank
        {
            get
            {
                foreach (DonationRank donationRank in new DonationRank[]
                {
                    DonationRank.Rank98,
                    DonationRank.Rank54,
                    DonationRank.Rank23,
                    DonationRank.Rank12,
                    DonationRank.Rank6
                })
                {
                    if (ModSecret.PotatoFeatures.Contains(donationRank.ToString()))
                    {
                        return donationRank;
                    }
                }
                return DonationRank.None;
            }
        }

        private static bool HasUpdatePermission
        {
            get
            {
                return ModSecret.CurrentRank >= DonationRank.Rank12 || ModSecret.PotatoFeatures.Contains("Update");
            }
        }

        private static List<string> PotatoFeatures
        {
            get
            {
                checked
                {
                    if (ModSecret._PotatoFeatures == null)
                    {
                        ModSecret._PotatoFeatures = new List<string>();
                        string[] array = Settings.Get<string>("Potatoes", null).ToString().Split('|', true);
                        for (int i = 0; i < array.Length; i++)
                        {
                            OneOf<List<string>, string> oneOf = ModSecret.CheckPotatoCode(array[i]);
                            if (oneOf.Is<List<string>>())
                            {
                                ModSecret._PotatoFeatures.AddRange(oneOf.As<List<string>>());
                            }
                        }
                        ModSecret._PotatoFeatures = ModSecret._PotatoFeatures.Distinct<string>().ToList<string>();
                    }
                    return ModSecret._PotatoFeatures;
                }
            }
        }

        private static OneOf<List<string>, string> CheckPotatoCode(string Key)
        {
            OneOf<List<string>, string> oneOf;
            if (string.IsNullOrEmpty(Key))
            {
                oneOf = "输入的土豆码为空。";
            }
            else if (Key.Length < 30 && Key.ContainsIgnoreCase(ModBase.Identify))
            {
                oneOf = string.Format("你输入的是识别码，不是土豆码。{0}要获取土豆码，请在爱发电私信发送【土豆 {1}】。", "\r\n", ModBase.Identify);
            }
            else if (Key.Length > 85 && Key.Length < 90 && Key.EndsWithF("=", false))
            {
                oneOf = string.Format("你输入的是解锁码，不是土豆码。{0}要获取土豆码，请在爱发电私信发送【土豆】。", "\r\n");
            }
            else if (Key.Length > 150 && Key.Length < 165 && Key.EndsWithF("11", false))
            {
                oneOf = string.Format("你输入的是更新密钥，不是土豆码。{0}要获取土豆码，请在爱发电私信发送【土豆】。", "\r\n");
            }
            else if (!Key.StartsWithF("CD", true) || !Key.Contains("#"))
            {
                oneOf = string.Format("你输入的不是土豆码……有一说一，这是啥玩意儿？{0}要获取土豆码，请在爱发电私信发送【土豆 {1}】。", "\r\n", ModBase.Identify);
            }
            else if (Key[2] > '1')
            {
                oneOf = string.Format("这个土豆码得在新版 PCL 上才能使用。{0}请更新 PCL，或是在爱发电私信发送【下载】，然后在新下载的 PCL 上输入土豆码。", "\r\n");
            }
            else if ('1' > Key[3])
            {
                oneOf = string.Format("这个土豆码不太新鲜，已经过期了。{0}请在爱发电私信重新发送【土豆 {1}】，获取更新鲜的土豆码。", "\r\n", ModBase.Identify);
            }
            else if (!Key.StartsWithF("CD", false))
            {
                oneOf = string.Format("土豆码区分大小写，但你把一些大写的东西输成小写的了。{0}你可以选中文本，然后右键复制粘贴，不必手动输入。", "\r\n");
            }
            else
            {
                try
                {
                    string[] array = Key.Substring(4).Replace("p", "=").Replace("%", "p")
                        .Split(new char[] { '#' });
                    string @string = Encoding.UTF8.GetString(Convert.FromBase64String(array[1]));
                    CryptographyUtils.EcdsaVerify(ModBase.Identify + Conversions.ToString(Key[2]) + Conversions.ToString(Key[3]) + @string, array[0], "RUNTMSAAAAC4QTUNAewh23Q4Q6koHkyIrDIIZUSbua23sf2DiZmIRwSzadISDRyTVTbuWniH3KR7rKj8XBsabms1be6i3c+S");
                    oneOf = @string.Split(',', true).ToList<string>();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, string.Format("土豆码校验失败（{0}）", Key), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    oneOf = string.Format("土豆码有误，请检查：{0}- 私信显示的识别码应当为 {1}，如果不是，请在私信发送【土豆 {2}】{3}- 你输入的以下土豆码是否与私信中的一致：{4}{5}{6}{7}错误原因：{8}", new object[]
                    {
                        "\r\n",
                        ModBase.Identify,
                        ModBase.Identify,
                        "\r\n",
                        "\r\n",
                        Key,
                        "\r\n",
                        "\r\n",
                        ex.Message
                    });
                }
            }
            return oneOf;
        }

        public static void InputPotatoCode(bool IsUpdating)
        {
            string text;
            if (IsUpdating)
            {
                if (ModMain.OurMsgBox(string.Format("请点击【复制命令】，然后将复制的命令粘贴到爱发电私信，获取土豆码。{0}输入土豆码后，就能继续更新啦！", "\r\n"), "验证爱发电赞助", "输入土豆码", "复制命令", "取消", false, true, false, null, (ModSecret._Closure___._I62_0 == null) ? (ModSecret._Closure___._I62_0 = new Action(ModSecret._Closure___._I._Lambda___62_0)) : ModSecret._Closure___._I62_0, null) == 3)
                {
                    return;
                }
                text = ModMain.OurMsgBoxInput("输入土豆码", string.Format("请在【爱发电】向龙腾猫跃发送【土豆 {0}】，然后输入自动回复的土豆码以继续更新。{1}自动回复可能比较慢，发送后等一会儿刷新一下网页应该就有了。", ModBase.Identify, "\r\n"), null, new Collection<Validate>
                {
                    new ValidateNullOrWhiteSpace()
                }, "", "确定", "取消", false);
            }
            else
            {
                text = ModMain.OurMsgBoxInput("输入土豆码", string.Format("请输入你获得的土豆码。{0}你的识别码：{1}", "\r\n", ModBase.Identify), null, new Collection<Validate>
                {
                    new ValidateNullOrWhiteSpace()
                }, "", "确定", "取消", false);
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ReplaceLineEndings("", false).Between("：", "在", false).Trim(new char[] { ' ', '【', '】' });
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Logger.Info(string.Format("输入的土豆码：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    if (!IsUpdating && checked(Operators.CompareString(text, "PuzzleUnlock" + ModBase.Identify.Substring(ModBase.Identify.Count<char>() - 4), false) == 0 || Operators.CompareString(text, "PuzzleUnlock" + ModBase.Identify.Substring(ModBase.Identify.Count<char>() - 5), false) == 0 || Operators.CompareString(text, "PuzzleUnlock" + ModBase.Identify.Substring(ModBase.Identify.Count<char>() - 6), false) == 0))
                    {
                        ModMain.OurMsgBox(string.Format("隐藏主题 极客蓝 已解锁！{0}难道，这就是终点了？还是说……？", "\r\n"), "解锁新隐藏主题！", "确定", "", "", false, true, false, null, null, null);
                        ModSecret.ThemeUnlock(11, true, null);
                        return;
                    }
                    try
                    {
                        OneOf<List<string>, string> oneOf = ModSecret.CheckPotatoCode(text);
                        if (oneOf.Is<string>())
                        {
                            throw new Exception(oneOf.As<string>());
                        }
                        List<string> list = oneOf.As<List<string>>();
                        Logger.Info(string.Format("土豆码包含的特性：{0}", list.Join(", ")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        if (list.Any<string>((ModSecret._Closure___._I62_1 == null) ? (ModSecret._Closure___._I62_1 = new Func<string, bool>(ModSecret._Closure___._I._Lambda___62_1)) : ModSecret._Closure___._I62_1))
                        {
                            List<string> list2 = Settings.Get<string>("Potatoes", null).ToString().Split('|', true)
                                .ToList<string>();
                            list2.Add(text);
                            Settings.Set("Potatoes", list2.Join("|"), null);
                            ModSecret._PotatoFeatures = null;
                        }
                        if (IsUpdating)
                        {
                            if (!ModSecret.HasUpdatePermission)
                            {
                                throw new Exception("这个土豆码里不包含更新权限……");
                            }
                        }
                        else
                        {
                            ModMain.Hint("已成功解锁！", ModMain.HintType.Green, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info(string.Format("土豆码无效：{0}", ex.Message), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        if (ModMain.OurMsgBox(ex.Message, "错误", "重新输入", "取消", "", true, true, false, null, null, null) == 1)
                        {
                            ModSecret.InputPotatoCode(IsUpdating);
                        }
                    }
                }
            }
        }

        internal static void GeneratePotatoCode()
        {
        }

        internal static string GetIdentify()
        {
            string text;
            try
            {
                if (ModBase.ApplicationStartTick < 1L)
                {
                    text = "0000-0000-0000-0000";
                }
                else
                {
                    string text2;
                    try
                    {
                        text2 = RegistryUtils.TryRead(string.Format("HKEY_LOCAL_MACHINE\\SYSTEM\\Har{0}wareConfig", "D".Lower()), "LastConfig", "Unknown").ToString().Upper()
                            .Trim(new char[] { '{', '}' });
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "获取主板标识码失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        text2 = "Unknown";
                    }
                    string text3;
                    try
                    {
                        text3 = Settings.Get<string>("Identify", null);
                    }
                    catch (Exception ex2)
                    {
                        text3 = "";
                    }
                    if (text3.Length < 3)
                    {
                        text3 = Conversions.ToString(ModBase.GetTimeMs()) + Conversions.ToString(OurWpfExtension.Computer.Info.AvailablePhysicalMemory);
                        Settings.Set("Identify", text3, null);
                    }
                    string text4 = (text2 + text3).GetStableHashCode().ToString("X").EnsureLength('7', 16);
                    text = string.Format("{0}-{1}-{2}-{3}", new object[]
                    {
                        text4.Substring(4, 4),
                        text4.Substring(12, 4),
                        text4.Substring(0, 4),
                        text4.Substring(8, 4)
                    });
                }
            }
            catch (Exception ex3)
            {
                Logger.Error(ex3, string.Format("PCL 无法获取设备识别码，这可能会导致部分设置无法正常存储。{0}{1}详细的错误信息", "\r\n", "\r\n"), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                text = "0000-0000-0000-0000";
            }
            return text;
        }

        public const string VersionBranchMain = "Official";

        public const string RegFolder = "PCL";

        public const string OAuthClientId = "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1";


        public const bool RestrictMcDownload = true;

        public static ModBase.OurColor Color1 = new ModBase.OurColor(52.0, 61.0, 74.0);

        public static ModBase.OurColor Color2 = new ModBase.OurColor(11.0, 91.0, 203.0);

        public static ModBase.OurColor Color3 = new ModBase.OurColor(19.0, 112.0, 243.0);

        public static ModBase.OurColor Color4 = new ModBase.OurColor(72.0, 144.0, 245.0);

        public static ModBase.OurColor Color5 = new ModBase.OurColor(150.0, 192.0, 249.0);

        public static ModBase.OurColor Color6 = new ModBase.OurColor(213.0, 230.0, 253.0);

        public static ModBase.OurColor Color7 = new ModBase.OurColor(222.0, 236.0, 253.0);

        public static ModBase.OurColor Color8 = new ModBase.OurColor(234.0, 242.0, 254.0);

        public static ModBase.OurColor ColorBg0 = new ModBase.OurColor(150.0, 192.0, 249.0);

        public static ModBase.OurColor ColorBg1 = new ModBase.OurColor(190.0, ModSecret.Color7);

        public static ModBase.OurColor ColorGray1 = new ModBase.OurColor(64.0, 64.0, 64.0);

        public static ModBase.OurColor ColorGray2 = new ModBase.OurColor(115.0, 115.0, 115.0);

        public static ModBase.OurColor ColorGray3 = new ModBase.OurColor(140.0, 140.0, 140.0);

        public static ModBase.OurColor ColorGray4 = new ModBase.OurColor(166.0, 166.0, 166.0);

        public static ModBase.OurColor ColorGray5 = new ModBase.OurColor(204.0, 204.0, 204.0);

        public static ModBase.OurColor ColorGray6 = new ModBase.OurColor(235.0, 235.0, 235.0);

        public static ModBase.OurColor ColorGray7 = new ModBase.OurColor(240.0, 240.0, 240.0);

        public static ModBase.OurColor ColorGray8 = new ModBase.OurColor(245.0, 245.0, 245.0);

        public static ModBase.OurColor ColorSemiTransparent = new ModBase.OurColor(1.0, ModSecret.Color8);

        public static int ThemeNow = -1;

        public static int ColorHue = 210;

        public static int ColorSat = 85;

        public static int ColorLightAdjust = 0;

        public static OneOf<int, int[]> ColorHueTopbarDelta = 0;

        public static int ThemeDontClick = 0;

        private static bool IsThemeColorfulLoaded = false;

        private static int ColorHueDelta = 1;


        internal static bool IsUpdateWaitingRestart = false;

        private static bool IsUpdateRestartWaitingTasks = false;


        public static JObject ServerConfig;

        public static ModLoader.LoaderTask<int, int> ServerLoader = new ModLoader.LoaderTask<int, int>("PCL 配置更新", new Action<ModLoader.LoaderTask<int, int>>(ModSecret.ServerSub), null, ThreadPriority.BelowNormal)
        {
            ReloadTimeout = 3600000
        };

        private static List<string> _PotatoFeatures = null;

        private const string EcdsaPublicKey = "RUNTMSAAAAC4QTUNAewh23Q4Q6koHkyIrDIIZUSbua23sf2DiZmIRwSzadISDRyTVTbuWniH3KR7rKj8XBsabms1be6i3c+S";




        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal void _Lambda___36_0()
            {
                if (ModMain.FrmMain.IsLoaded)
                {
                    LinearGradientBrush linearGradientBrush = new LinearGradientBrush
                    {
                        EndPoint = new Point(1.0, 0.0),
                        StartPoint = new Point(0.0, 0.0)
                    };
                    checked
                    {
                        if (ModSecret.ThemeNow == 5)
                        {
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.0,
                                Color = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 25.0)
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.5,
                                Color = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 15.0)
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 1.0,
                                Color = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 25.0)
                            });
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush;
                            ModMain.FrmMain.PanTitle.Background.Freeze();
                        }
                        else if (ModSecret.ThemeNow != 12 && ModSecret.ThemeDontClick != 2)
                        {
                            int[] array = ModSecret.ColorHueTopbarDelta.Switch<int[]>((ModSecret._Closure___._I36_1 == null) ? (ModSecret._Closure___._I36_1 = new Func<int, int[]>(ModSecret._Closure___._I._Lambda___36_1)) : ModSecret._Closure___._I36_1, (ModSecret._Closure___._I36_2 == null) ? (ModSecret._Closure___._I36_2 = new Func<int[], int[]>(ModSecret._Closure___._I._Lambda___36_2)) : ModSecret._Closure___._I36_2);
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.0,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue + array[0]), (double)ModSecret.ColorSat, (double)(48 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.5,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue + array[1]), (double)ModSecret.ColorSat, (double)(54 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 1.0,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue + array[2]), (double)ModSecret.ColorSat, (double)(48 + ModSecret.ColorLightAdjust))
                            });
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush;
                            ModMain.FrmMain.PanTitle.Background.Freeze();
                        }
                        else
                        {
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.0,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue - 21), (double)ModSecret.ColorSat, (double)(53 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.33,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue - 7), (double)ModSecret.ColorSat, (double)(47 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.67,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue + 7), (double)ModSecret.ColorSat, (double)(47 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 1.0,
                                Color = new ModBase.OurColor().FromHSL2((double)(ModSecret.ColorHue + 21), (double)ModSecret.ColorSat, (double)(53 + ModSecret.ColorLightAdjust))
                            });
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush;
                        }
                        if (ModSecret.ThemeNow >= 5 && ModSecret.ThemeNow != 14)
                        {
                            ModMain.FrmMain.ImgTitle.Source = string.Format("{0}Themes/{1}.png", ModBase.PathImage, ModSecret.ThemeNow);
                        }
                        else
                        {
                            ModMain.FrmMain.ImgTitle.Source = null;
                        }
                        ModMain.FrmMain.ImgTitle.Opacity = ((ModSecret.ThemeNow == 13) ? 0.25 : 0.5);
                    }
                    if (Settings.Get<bool>("UiBackgroundColorful", null))
                    {
                        linearGradientBrush = new LinearGradientBrush
                        {
                            EndPoint = new Point(0.1, 1.0),
                            StartPoint = new Point(0.9, 0.0)
                        };
                        linearGradientBrush.GradientStops.Add(new GradientStop
                        {
                            Offset = -0.1,
                            Color = new ModBase.OurColor().FromHSL2((double)(checked(ModSecret.ColorHue - 15)), (double)ModSecret.ColorSat * 0.8, 91.0)
                        });
                        linearGradientBrush.GradientStops.Add(new GradientStop
                        {
                            Offset = 0.4,
                            Color = new ModBase.OurColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat * 0.8, 91.0)
                        });
                        linearGradientBrush.GradientStops.Add(new GradientStop
                        {
                            Offset = 1.1,
                            Color = new ModBase.OurColor().FromHSL2((double)(checked(ModSecret.ColorHue + 15)), (double)ModSecret.ColorSat * 0.8, 91.0)
                        });
                        ModMain.FrmMain.PanForm.Background = linearGradientBrush;
                    }
                    else
                    {
                        ModMain.FrmMain.PanForm.Background = new ModBase.OurColor(245.0, 245.0, 245.0);
                    }
                    ModMain.FrmMain.PanForm.Background.Freeze();
                }
            }

            internal int[] _Lambda___36_1(int d)
            {
                return new int[]
                {
                    checked(0 - d),
                    0,
                    d
                };
            }

            internal int[] _Lambda___36_2(int[] d)
            {
                return d;
            }


            internal string _Lambda___43_1(string u)
            {
                return ModMain.ArgumentReplace(u, new Func<string, string>(WebUtility.HtmlEncode), true);
            }


            internal void _Lambda___62_0()
            {
                ModBase.ClipboardSet("土豆 " + ModBase.Identify, false);
                ModBase.OpenWebsite("https://meloong.com/afd/message/df2dfad2960911e89c5252540025c377?potato");
            }

            internal bool _Lambda___62_1(string f)
            {
                return !ModSecret.PotatoFeatures.Contains(f);
            }

            public static readonly ModSecret._Closure___ _I = new ModSecret._Closure___();

            public static Func<int, int[]> _I36_1;

            public static Func<int[], int[]> _I36_2;

            public static Action _I36_0;


            public static Func<string, string> _I43_1;


            public static Action _I62_0;

            public static Func<string, bool> _I62_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___37_0
        {
            public _Closure___37_0(ModSecret._Closure___37_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_EffectSetup = arg0._VB_Local_EffectSetup;
                }
            }

            internal void _Lambda___0()
            {
                checked
                {
                    try
                    {
                        int num = Settings.Get<int>("UiLauncherTheme", null);
                        if (!ModSecret.ThemeCheckOne(num))
                        {
                            Logger.Warn(string.Format("检测到无效主题：{0}，已重置为默认主题", num), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                            Settings.Set("UiLauncherTheme", 0, null);
                        }
                        if (this._VB_Local_EffectSetup && ModMain.FrmSetupUI != null)
                        {
                            List<int> list = new List<int>();
                            int num2 = 0;
                            do
                            {
                                if (ModSecret.ThemeCheckOne(num2))
                                {
                                    list.Add(num2);
                                }
                                num2++;
                            }
                            while (num2 <= 14);
                            list.Add(42);
                            ModAnimation.AniControlEnabled++;
                            if (list.Contains(5))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme5.Opacity = 1.0;
                                ModMain.FrmSetupUI.LabLauncherTheme5Unlock.Visibility = Visibility.Collapsed;
                                ModMain.FrmSetupUI.RadioLauncherTheme5Gray.Visibility = Visibility.Collapsed;
                            }
                            if (list.Contains(6))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme6.IsEnabled = true;
                            }
                            if (list.Contains(7))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme7.IsEnabled = true;
                            }
                            if (list.Contains(8))
                            {
                                ModMain.FrmSetupUI.LabLauncherTheme8Copy.Visibility = Visibility.Collapsed;
                                ModMain.FrmSetupUI.RadioLauncherTheme8.IsEnabled = true;
                            }
                            if (list.Contains(9))
                            {
                                ModMain.FrmSetupUI.LabLauncherTheme9Copy.Visibility = Visibility.Collapsed;
                                ModMain.FrmSetupUI.RadioLauncherTheme9.IsEnabled = true;
                            }
                            if (list.Contains(10))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme10.IsEnabled = true;
                            }
                            if (list.Contains(11))
                            {
                                ModMain.FrmSetupUI.LabLauncherTheme11Click.Visibility = Visibility.Collapsed;
                                ModMain.FrmSetupUI.RadioLauncherTheme11.IsEnabled = true;
                            }
                            else if (!Information.IsNothing(ModMain.FrmSetupUI) && !Information.IsNothing(ModMain.FrmSetupUI.RadioLauncherTheme11) && ModMain.FrmSetupUI.RadioLauncherTheme11.IsLoaded)
                            {
                                int num3 = 0;
                                try
                                {
                                    foreach (int num4 in list)
                                    {
                                        if (num4 >= 5 && num4 <= 14)
                                        {
                                            num3++;
                                        }
                                    }
                                }
                                finally
                                {
                                    List<int>.Enumerator enumerator = default;
                                    ((IDisposable)enumerator).Dispose();
                                }
                                if (num3 < 3)
                                {
                                    ModMain.FrmSetupUI.LabLauncherTheme11Click.ToolTip = "需要解锁三个隐藏主题";
                                }
                                else
                                {
                                    ModMain.FrmSetupUI.LabLauncherTheme11Click.ToolTip = "点击打开解密游戏入口";
                                }
                            }
                            if (list.Contains(12))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme12.IsEnabled = true;
                            }
                            if (list.Contains(13))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme13.IsEnabled = true;
                            }
                            if (list.Contains(14))
                            {
                                ModMain.FrmSetupUI.RadioLauncherTheme14.IsEnabled = true;
                            }
                            ModAnimation.AniControlEnabled--;
                        }
                    }
                    catch (Exception ex)
                    {
                        if ((ex.InnerException ?? ex) is FormatException)
                        {
                            Logger.Error(ex, "解锁的主题列表存档已损坏，主题解锁已被重置", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                            Settings.Set("UiLauncherThemeHide2", "", null);
                        }
                        else
                        {
                            Logger.Error(ex, "检查主题失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        }
                    }
                }
            }

            public bool _VB_Local_EffectSetup;
        }



        [CompilerGenerated]
        private sealed class _Closure___46_0
        {
            public _Closure___46_0(ModSecret._Closure___46_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_NewFilePath = arg0._VB_Local_NewFilePath;
                }
            }

            internal string _Lambda___0(string Reason)
            {
                string[] array = new string[5];
                array[0] = string.Format("由于{0}，PCL 无法完成更新。{1}请依次尝试：{2}", Reason, "\r\n", "\r\n");
                array[1] = ((PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Paths.Base) || PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Paths.Base)) ? " - 将 PCL 文件移动到桌面、文档、C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）\r\n" : "");
                int num = 2;
                char? diskName = PathUtils.GetDiskName(Paths.Base);
                array[num] = (((diskName != null) ? new bool?(diskName.GetValueOrDefault() == 'C') : null).GetValueOrDefault() ? " - 将 PCL 文件移动到 C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）\r\n" : "");
                array[3] = ((!WindowsUtils.HasAdminRole()) ? " - 右键 PCL，选择以管理员身份运行\r\n" : "");
                array[4] = string.Format(" - 手动剪切 PCL 文件夹中的新版本程序，覆盖原程序：{0}   将位于 {1}{2}   的程序{3}   剪切到 {4} 并覆盖即可。", new object[]
                {
                    "\r\n",
                    ModBase.PathExe,
                    "\r\n",
                    "\r\n",
                    this._VB_Local_NewFilePath
                });
                return string.Concat(array);
            }

            public string _VB_Local_NewFilePath;
        }
    }
}
