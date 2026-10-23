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
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using PCL.My;
using PCLCS;

namespace PCL
{
    [StandardModule]
    internal sealed class ModSecret
    {
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
                    // The original CurseForge API key was removed before publishing. Supply your own (base64-encoded) here.
                    Req.Headers.Add("x-api-key", Encoding.Default.GetString(Convert.FromBase64String("")));
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
                            ModSecret.Color1 = unchecked(new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat * 0.2, 25.0 + (double)ModSecret.ColorLightAdjust * 0.3));
                            ModSecret.Color2 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, (double)(45 + ModSecret.ColorLightAdjust));
                            ModSecret.Color3 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, (double)(55 + ModSecret.ColorLightAdjust));
                            ModSecret.Color4 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, (double)(65 + ModSecret.ColorLightAdjust));
                        }
                        ModSecret.Color5 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 80.0 + (double)ModSecret.ColorLightAdjust * 0.4);
                        ModSecret.Color6 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 91.0 + (double)ModSecret.ColorLightAdjust * 0.1);
                        ModSecret.Color7 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 95.0);
                        ModSecret.Color8 = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 97.0);
                        ModSecret.ColorBg0 = ModSecret.Color4 * 0.4 + ModSecret.Color5 * 0.4 + ModSecret.ColorGray4 * 0.2;
                        ModSecret.ColorBg1 = new ModBase.MyColor(190.0, ModSecret.Color7);
                        ModSecret.ColorSemiTransparent = new ModBase.MyColor(1.0, ModSecret.Color8);
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
                        Application.Current.Resources["ColorObject1"] = ModSecret.Color1;
                        Application.Current.Resources["ColorObject2"] = ModSecret.Color2;
                        Application.Current.Resources["ColorObject3"] = ModSecret.Color3;
                        Application.Current.Resources["ColorObject4"] = ModSecret.Color4;
                        Application.Current.Resources["ColorObject5"] = ModSecret.Color5;
                        Application.Current.Resources["ColorObject6"] = ModSecret.Color6;
                        Application.Current.Resources["ColorObject7"] = ModSecret.Color7;
                        Application.Current.Resources["ColorObject8"] = ModSecret.Color8;
                        Application.Current.Resources["ColorObjectBg0"] = ModSecret.ColorBg0;
                        Application.Current.Resources["ColorObjectBg1"] = ModSecret.ColorBg1;
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
            ModBase.RunInUi((ModSecret._Closure$__.$I36-0 == null) ? (ModSecret._Closure$__.$I36-0 = delegate
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
                                Color = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 25.0)
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.5,
                                Color = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 15.0)
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 1.0,
                                Color = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat, 25.0)
                            });
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush;
                            ModMain.FrmMain.PanTitle.Background.Freeze();
                        }
                        else if (ModSecret.ThemeNow != 12 && ModSecret.ThemeDontClick != 2)
                        {
                            int[] array = ModSecret.ColorHueTopbarDelta.Switch<int[]>((ModSecret._Closure$__.$I36-1 == null) ? (ModSecret._Closure$__.$I36-1 = (int d) => new int[]
                            {
                                0 - d,
                                0,
                                d
                            }) : ModSecret._Closure$__.$I36-1, (ModSecret._Closure$__.$I36-2 == null) ? (ModSecret._Closure$__.$I36-2 = (int[] d) => d) : ModSecret._Closure$__.$I36-2);
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.0,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue + array[0]), (double)ModSecret.ColorSat, (double)(48 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.5,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue + array[1]), (double)ModSecret.ColorSat, (double)(54 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 1.0,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue + array[2]), (double)ModSecret.ColorSat, (double)(48 + ModSecret.ColorLightAdjust))
                            });
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush;
                            ModMain.FrmMain.PanTitle.Background.Freeze();
                        }
                        else
                        {
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.0,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue - 21), (double)ModSecret.ColorSat, (double)(53 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.33,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue - 7), (double)ModSecret.ColorSat, (double)(47 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 0.67,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue + 7), (double)ModSecret.ColorSat, (double)(47 + ModSecret.ColorLightAdjust))
                            });
                            linearGradientBrush.GradientStops.Add(new GradientStop
                            {
                                Offset = 1.0,
                                Color = new ModBase.MyColor().FromHSL2((double)(ModSecret.ColorHue + 21), (double)ModSecret.ColorSat, (double)(53 + ModSecret.ColorLightAdjust))
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
                            Color = new ModBase.MyColor().FromHSL2((double)(checked(ModSecret.ColorHue - 15)), (double)ModSecret.ColorSat * 0.8, 91.0)
                        });
                        linearGradientBrush.GradientStops.Add(new GradientStop
                        {
                            Offset = 0.4,
                            Color = new ModBase.MyColor().FromHSL2((double)ModSecret.ColorHue, (double)ModSecret.ColorSat * 0.8, 91.0)
                        });
                        linearGradientBrush.GradientStops.Add(new GradientStop
                        {
                            Offset = 1.1,
                            Color = new ModBase.MyColor().FromHSL2((double)(checked(ModSecret.ColorHue + 15)), (double)ModSecret.ColorSat * 0.8, 91.0)
                        });
                        ModMain.FrmMain.PanForm.Background = linearGradientBrush;
                    }
                    else
                    {
                        ModMain.FrmMain.PanForm.Background = new ModBase.MyColor(245.0, 245.0, 245.0);
                    }
                    ModMain.FrmMain.PanForm.Background.Freeze();
                }
            }) : ModSecret._Closure$__.$I36-0, false);
        }

        internal static void ThemeCheckAll(bool EffectSetup)
        {
            ModSecret._Closure$__37-0 CS$<>8__locals1 = new ModSecret._Closure$__37-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_EffectSetup = EffectSetup;
            ModBase.RunInUi(checked(delegate
            {
                try
                {
                    int num = Settings.Get<int>("UiLauncherTheme", null);
                    if (!ModSecret.ThemeCheckOne(num))
                    {
                        Logger.Warn(string.Format("检测到尚未解锁的主题：{0}，已重置为默认主题", num), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        Settings.Set("UiLauncherTheme", 0, null);
                    }
                    if (CS$<>8__locals1.$VB$Local_EffectSetup && ModMain.FrmSetupUI != null)
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
                                List<int>.Enumerator enumerator;
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
            }), false);
        }

        internal static bool ThemeCheckOne(int Id)
        {
            bool flag;
            if (new int[] { 0, 1, 2, 3, 4, 42 }.Contains(Id))
            {
                flag = true;
            }
            else if (ModSecret.PotatoFeatures.Contains("Theme" + Conversions.ToString(Id)))
            {
                flag = true;
            }
            else
            {
                int num = Id;
                if (num != 8)
                {
                    if (num != 14)
                    {
                        if (Settings.Get<string>("UiLauncherThemeHide", null).Contains("7"))
                        {
                            ModSecret.ThemeUnlock(7, false, null);
                        }
                        flag = Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false)
                            .Contains(Id.ToString());
                    }
                    else
                    {
                        flag = Enumerable.Range(5, 9).Count<int>(new Func<int, bool>(ModSecret.ThemeCheckOne)) >= 5;
                    }
                }
                else
                {
                    flag = ModSecret.CurrentRank >= DonationRank.Rank23;
                }
            }
            return flag;
        }

        internal static bool ThemeUnlock(int Id, bool ShowDoubleHint = true, string UnlockHint = null)
        {
            bool flag;
            if (ModMain.FrmMain == null)
            {
                flag = false;
            }
            else
            {
                if (Id == 8)
                {
                    throw new Exception("不可使用此方式解锁秋仪金！");
                }
                List<string> list = new List<string>(Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false));
                if (list.Contains(Id.ToString()))
                {
                    if (ShowDoubleHint)
                    {
                        ModMain.Hint("该隐藏主题已被解锁！", ModMain.HintType.Blue, true);
                    }
                    flag = false;
                }
                else
                {
                    Logger.Info(string.Format("解锁隐藏主题：{0}", Id), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    if (UnlockHint != null)
                    {
                        ModMain.Hint(UnlockHint, ModMain.HintType.Green, false);
                    }
                    list.Add(Id.ToString());
                    Settings.Set("UiLauncherThemeHide2", list.Join("|"), null);
                    ModSecret.ThemeCheckAll(true);
                    flag = true;
                }
            }
            return flag;
        }

        internal static void UpdateCheckByButton()
        {
            if (ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init, ref flag);
                if (ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State == 0)
                {
                    ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State = 2;
                    ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate = false;
                }
                else if (ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init);
                }
            }
            if (ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate)
            {
                ModMain.Hint("正在检查更新，请稍候！", ModMain.HintType.Blue, true);
                return;
            }
            ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate = true;
            ModBase.RunInThread((ModSecret._Closure$__.$I40-0 == null) ? (ModSecret._Closure$__.$I40-0 = delegate
            {
                try
                {
                    ModMain.Hint("正在检查更新，请稍候！", ModMain.HintType.Blue, true);
                    ModSecret.ServerLoader.WaitForExit(null, null, true, default(CancellationToken), null);
                    bool? flag2 = PageSetupSystem.IsLauncherNewest();
                    if (flag2 == null)
                    {
                        ModMain.Hint("连接 PCL 服务器失败，请确认系统时间是否准确，尝试将 PCL 加入杀毒软件或防火墙白名单，然后重启 PCL！", ModMain.HintType.Red, true);
                    }
                    else if (flag2.GetValueOrDefault())
                    {
                        ModMain.Hint(string.Format("已经是最新{0} {1}，不需要更新啦，可以直接使用！", "快照版", "2.99.0.0"), ModMain.HintType.Green, true);
                    }
                    else
                    {
                        if (!ModSecret.HasUpdatePermission)
                        {
                            ModSecret.InputPotatoCode(true);
                            if (!ModSecret.HasUpdatePermission)
                            {
                                return;
                            }
                        }
                        else if (ModMain.MyMsgBox("发现启动器更新，是否立即下载？", "发现更新！", "更新！冲！", "取消", "", false, true, false, null, null, null) == 2)
                        {
                            return;
                        }
                        ModSecret.UpdateStart(false, null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "尝试手动开始更新失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                }
                finally
                {
                    ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate = false;
                }
            }) : ModSecret._Closure$__.$I40-0);
        }

        private static void UpdateStart(bool Silent, IEnumerable<string> UrlsOverride = null)
        {
            ModSecret._Closure$__43-0 CS$<>8__locals1 = new ModSecret._Closure$__43-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_UrlsOverride = UrlsOverride;
            if (ModSecret.IsUpdating)
            {
                if (!Silent)
                {
                    ModMain.Hint("PCL 正在下载更新，更新结束时将自动重启，请稍候！", ModMain.HintType.Blue, true);
                    ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate = false;
                }
                if (ModSecret.IsUpdateWaitingRestart)
                {
                    ModSecret.UpdateRestart(true);
                    return;
                }
            }
            else
            {
                ModSecret.IsUpdating = true;
                ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate = Silent;
                if (!ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate)
                {
                    ModMain.Hint("PCL 正在下载更新，更新结束时将自动重启，请稍候！", ModMain.HintType.Blue, true);
                }
                try
                {
                    ModSecret._Closure$__43-1 CS$<>8__locals2 = new ModSecret._Closure$__43-1(CS$<>8__locals2);
                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                    CS$<>8__locals2.$VB$Local_Urls = null;
                    CS$<>8__locals2.$VB$Local_Extension = null;
                    CS$<>8__locals2.$VB$Local_LocalPath = null;
                    CS$<>8__locals2.$VB$Local_Loader = new ModLoader.LoaderCombo<string>("启动器更新", new ModLoader.LoaderBase[]
                    {
                        new ModLoader.LoaderTask<string, List<ModNet.NetFile>>("获取更新信息", delegate(ModLoader.LoaderTask<string, List<ModNet.NetFile>> Task)
                        {
                            if (ModSecret.ServerLoader.State == ModBase.LoadState.Loading)
                            {
                                ModSecret.ServerLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                            }
                            if (CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_UrlsOverride != null)
                            {
                                CS$<>8__locals2.$VB$Local_Urls = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_UrlsOverride.ToList<string>();
                            }
                            else
                            {
                                JObject serverConfig = ModSecret.ServerConfig;
                                bool flag;
                                if (serverConfig == null)
                                {
                                    flag = null != null;
                                }
                                else
                                {
                                    JToken jtoken = serverConfig["Update"];
                                    flag = ((jtoken != null) ? jtoken[ModBase.BuildTypes.Snapshot.ToString()] : null) != null;
                                }
                                if (flag)
                                {
                                    JObject jobject = (JObject)ModSecret.ServerConfig["Update"][ModBase.BuildTypes.Snapshot.ToString()];
                                    if (jobject.ContainsKey("PatchUrls") && ModSecret.UpdateMark.StartsWithF("Empty", false))
                                    {
                                        CS$<>8__locals2.$VB$Local_Urls = jobject["PatchUrls"].ToObject<List<string>>();
                                    }
                                    else if (jobject.ContainsKey("DownloadUrls"))
                                    {
                                        CS$<>8__locals2.$VB$Local_Urls = jobject["DownloadUrls"].ToObject<List<string>>();
                                    }
                                }
                            }
                            if (CS$<>8__locals2.$VB$Local_Urls == null || !CS$<>8__locals2.$VB$Local_Urls.Any<string>())
                            {
                                Logger.Error("更新失败：服务器没有提供有效的更新包下载链接，或无法连接到服务器！", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                                ModSecret.IsUpdating = false;
                                return;
                            }
                            CS$<>8__locals2.$VB$Local_Urls = CS$<>8__locals2.$VB$Local_Urls.Select<string, string>((ModSecret._Closure$__.$I43-1 == null) ? (ModSecret._Closure$__.$I43-1 = (string u) => ModMain.ArgumentReplace(u, new Func<string, string>(WebUtility.HtmlEncode), true)) : ModSecret._Closure$__.$I43-1).ToList<string>();
                            CS$<>8__locals2.$VB$Local_Extension = "." + PathUtils.GetExtension(CS$<>8__locals2.$VB$Local_Urls.First<string>().BeforeLast("{CDN}", false));
                            CS$<>8__locals2.$VB$Local_LocalPath = string.Format("{0}{1}.{2}{3}", new object[]
                            {
                                ModMain.RequestTaskTempFolder(false),
                                "2.99.0.0",
                                0,
                                CS$<>8__locals2.$VB$Local_Extension
                            });
                            Logger.Info(string.Format("更新开始，静默：{0}，扩展名：{1}，下载目标：{2}，URL：{3}", new object[]
                            {
                                ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate,
                                CS$<>8__locals2.$VB$Local_Extension,
                                CS$<>8__locals2.$VB$Local_LocalPath,
                                CS$<>8__locals2.$VB$Local_Urls.Join("，")
                            }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                            Task.Output = new ModNet.NetFile[]
                            {
                                new ModNet.NetFile(CS$<>8__locals2.$VB$Local_Urls, CS$<>8__locals2.$VB$Local_LocalPath, null, false)
                            }.ToList<ModNet.NetFile>();
                        }, null, ThreadPriority.Normal)
                        {
                            ProgressWeight = 0.1
                        },
                        new ModNet.LoaderDownload("下载更新文件", new List<ModNet.NetFile>())
                        {
                            ProgressWeight = 1.0
                        },
                        new ModLoader.LoaderTask<string, int>("安装更新", delegate(ModLoader.LoaderTask<string, int> a0)
                        {
                            base._Lambda$__2();
                        }, null, ThreadPriority.Normal)
                        {
                            ProgressWeight = 0.1
                        }
                    });
                    CS$<>8__locals2.$VB$Local_Loader.OnStateChanged = delegate(ModLoader.LoaderBase a0)
                    {
                        base._Lambda$__3();
                    };
                    CS$<>8__locals2.$VB$Local_Loader.Start(null, false);
                    ModBase.RunInUi(delegate
                    {
                        if (!ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate)
                        {
                            ModLoader.LoaderTaskbarAdd<string>(CS$<>8__locals2.$VB$Local_Loader);
                            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                        }
                    }, false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "开始启动器更新失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    ModSecret.IsUpdating = false;
                }
            }
        }

        public static void UpdateRestart(bool TriggerRestartAndByEnd)
        {
            try
            {
                ModSecret.IsUpdateWaitingRestart = false;
                string text = Paths.Base + "PCL\\Plain Craft Launcher 2.exe";
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
                if (ModMain.MyMsgBox(string.Format("由于被 Windows 安全中心拦截，或者存在权限问题，导致 PCL 无法更新。{0}请将 PCL 所在文件夹加入白名单，或者手动用 {1}PCL\\Plain Craft Launcher 2.exe 替换当前文件！", "\r\n", Paths.Base), "更新失败", "查看帮助", "确定", "", true, true, false, null, null, null) == 1)
                {
                    CustomEvent.Raise(CustomEvent.EventType.打开帮助, "启动器/Microsoft Defender 添加排除项.json");
                }
            }
        }

        public static void UpdateReplace(int ProcessId, string OldFileName, string NewFileName, bool TriggerRestart)
        {
            ModSecret._Closure$__46-0 CS$<>8__locals1 = new ModSecret._Closure$__46-0(CS$<>8__locals1);
            try
            {
                Process.GetProcessById(ProcessId).Kill();
            }
            catch (Exception ex)
            {
            }
            string text = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(OldFileName));
            CS$<>8__locals1.$VB$Local_NewFilePath = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(NewFileName));
            Exception ex2 = null;
            int num = 0;
            checked
            {
                do
                {
                    try
                    {
                        FileUtils.Delete(text, false);
                        FileUtils.Delete(CS$<>8__locals1.$VB$Local_NewFilePath, false);
                        if (!FileUtils.Exists(text) && !FileUtils.Exists(CS$<>8__locals1.$VB$Local_NewFilePath))
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
                VB$AnonymousDelegate_11<string, string> vb$AnonymousDelegate_ = delegate(string Reason)
                {
                    string[] array = new string[5];
                    array[0] = string.Format("由于{0}，PCL 无法完成更新。{1}请依次尝试：{2}", Reason, "\r\n", "\r\n");
                    array[1] = ((PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Paths.Base) || PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Paths.Base)) ? " - 将 PCL 文件移动到桌面、文档、C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）\r\n" : "");
                    int num2 = 2;
                    char? diskName = PathUtils.GetDiskName(Paths.Base);
                    array[num2] = (((diskName != null) ? new bool?(diskName.GetValueOrDefault() == 'C') : null).GetValueOrDefault() ? " - 将 PCL 文件移动到 C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）\r\n" : "");
                    array[3] = ((!WindowsUtils.HasAdminRole()) ? " - 右键 PCL，选择以管理员身份运行\r\n" : "");
                    array[4] = string.Format(" - 手动剪切 PCL 文件夹中的新版本程序，覆盖原程序：{0}   将位于 {1}{2}   的程序{3}   剪切到 {4} 并覆盖即可。", new object[]
                    {
                        "\r\n",
                        ModBase.PathExe,
                        "\r\n",
                        "\r\n",
                        CS$<>8__locals1.$VB$Local_NewFilePath
                    });
                    return string.Concat(array);
                };
                if (!FileUtils.Exists(text) && !FileUtils.Exists(CS$<>8__locals1.$VB$Local_NewFilePath))
                {
                    try
                    {
                        FileUtils.Copy(ModBase.PathExe, CS$<>8__locals1.$VB$Local_NewFilePath);
                    }
                    catch (UnauthorizedAccessException ex4)
                    {
                        Interaction.MsgBox(string.Format("{0}{1}{2}详细错误信息：{3}{4}", new object[]
                        {
                            vb$AnonymousDelegate_("复制新版本文件时权限不足"),
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
                            vb$AnonymousDelegate_("复制新版本文件时出错"),
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
                            ModBase.StartProcess(CS$<>8__locals1.$VB$Local_NewFilePath, "");
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
                        vb$AnonymousDelegate_("删除老版本文件时权限不足"),
                        "\r\n",
                        "\r\n",
                        "\r\n",
                        ex2.GetDisplay(true)
                    }), MsgBoxStyle.Critical, "更新失败");
                    return;
                }
                Interaction.MsgBox(string.Format("{0}{1}{2}详细错误信息：{3}{4}", new object[]
                {
                    vb$AnonymousDelegate_("删除老版本文件时出错"),
                    "\r\n",
                    "\r\n",
                    "\r\n",
                    ex2.GetDisplay(true)
                }), MsgBoxStyle.Critical, "更新失败");
            }
        }

        internal static void DownloadLatestPCL(ModLoader.LoaderBase LoaderToSyncProgress = null)
        {
            int num = (FileUtils.Exists(ModBase.PathTemp + "Latest.exe") ? Settings.Get<int>("SystemHighestSavedBetaVersionReg", null) : 0);
            int num2 = 0;
            string text = FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\\Notice.cfg", null, null) ?? "";
            if (text.Split("|", false).Count<string>() >= 3)
            {
                num2 = Conversions.ToInteger(text.Split("|", false)[2]);
            }
            if (num < num2)
            {
                Logger.Info(string.Format("需要下载最新正式版：{0} -> {1}", num, num2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                if (ModSecret.ServerConfig == null)
                {
                    ModSecret.ServerLoader.WaitForExit(null, null, ModSecret.ServerLoader.State == ModBase.LoadState.Finished, default(CancellationToken), null);
                }
                if (ModSecret.ServerConfig == null)
                {
                    throw new Exception("无法从服务器获取正式版 PCL 下载链接！");
                }
                ModNet.NetDownloadByLoader(ModSecret.ServerConfig["Update"]["Release"]["DownloadUrls"].Select<JToken, string>((ModSecret._Closure$__.$I47-0 == null) ? (ModSecret._Closure$__.$I47-0 = (JToken t) => t.ToString()) : ModSecret._Closure$__.$I47-0).ToList<string>(), ModBase.PathTemp + "Latest.zip", LoaderToSyncProgress, new FileChecker
                {
                    MinSize = 1048576L
                }, false);
                FileUtils.ExtractToDirectory(ModBase.PathTemp + "Latest.zip", ModBase.PathTemp, default(CancellationToken), null);
                FileUtils.Delete(ModBase.PathTemp + "Latest.zip", false);
                FileUtils.Move(ModBase.PathTemp + "Plain Craft Launcher 2.exe", ModBase.PathTemp + "Latest.exe", false);
                Settings.Set("SystemHighestSavedBetaVersionReg", num2, null);
            }
        }

        private static void ServerSub(ModLoader.LoaderTask<int, int> Loader)
        {
            try
            {
                if (FileUtils.Exists(Paths.Base + "PCL\\update.exe"))
                {
                    FileUtils.Delete(Paths.Base + "PCL\\update.exe", false);
                    Logger.Info("已清理更新缓存", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "清理更新缓存失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
            }
            int num = 0;
            string text = ModBase.PathTemp + "Cache\\ServerConfig.json";
            checked
            {
                try
                {
                    if (FileUtils.Exists(text))
                    {
                        ModSecret.ServerConfig = (JObject)FileUtils.ReadAsJson(text, null, null);
                        num = (int)Math.Round(ModBase.Val(ModSecret.ServerConfig["Version"]));
                        Logger.Info(string.Format("已读取本地缓存的配置：{0}", num), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2, "读取配置信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    ModSecret.ServerConfig = null;
                    FileUtils.Delete(text, false);
                }
                Loader.Progress = 0.2;
                int num2 = Settings.Get<int>("HintNotice", null);
                string text2 = ModBase.PathTemp + "Cache\\Notice.cfg";
                string text3;
                int num3;
                try
                {
                    text3 = ModNet.NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/notice.cfg{CDN}", null, null, null, null, "*/*", null, false, null, false);
                    num3 = (int)Math.Round(ModBase.Val(text3.Split("|", false)[0]));
                    if (num3 == 0)
                    {
                        throw new Exception("获取到的内容有误！（" + text3 + "）");
                    }
                    if (num3 > num2)
                    {
                        Logger.Info(string.Format("本地公告编号：{0}，服务器公告：{1}，需更新", num2, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    }
                    else
                    {
                        Logger.Info(string.Format("本地公告编号：{0}，服务器公告：{1}，无需更新", num2, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    }
                    FileUtils.Write(text2, text3, null);
                }
                catch (Exception ex3)
                {
                    if (ex3 is InvalidOperationException && ex3.Message.Contains("FIPS"))
                    {
                        if (ModMain.MyMsgBox("由于系统未启用 FIPS 兼容算法，PCL 可能无法正常运行。\r\n请按照教程启用该功能，然后重启 PCL。", "兼容性警告", "打开教程", "取消", "", true, true, false, null, null, null) == 1)
                        {
                            ModBase.OpenWebsite("https://blog.csdn.net/qq_37608398/article/details/81209922");
                        }
                    }
                    else
                    {
                        Logger.Warn(ex3, "获取 PCL 服务器状态失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        FileUtils.Delete(text2, false);
                    }
                    return;
                }
                Loader.Progress = 0.4;
                try
                {
                    double num4 = ModBase.Val(text3.Split("|", false)[4]);
                    if ((double)num < num4)
                    {
                        Logger.Info(string.Format("本地配置版本号：{0}，服务器配置版本号：{1}，需要更新", num, num4), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                        ModNet.NetDownloadByClient("https://pcl2-server-1253424809.file.myqcloud.com/ServerConfig.json{CDN}", text, false);
                        ModSecret.ServerConfig = (JObject)FileUtils.ReadAsJson(text, null, null);
                    }
                }
                catch (Exception ex4)
                {
                    Logger.Warn(ex4, "下载配置信息失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                }
                Loader.Progress = 0.6;
                string text4;
                try
                {
                    if (num3 <= num2 && FileUtils.Exists(ModBase.PathTemp + "Cache\\Notice.json"))
                    {
                        text4 = FileUtils.ReadAsString(ModBase.PathTemp + "Cache\\Notice.json", null, null);
                    }
                    else
                    {
                        text4 = ModNet.NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/notice.json{CDN}", null, null, null, null, "*/*", null, true, null, false);
                        FileUtils.Write(ModBase.PathTemp + "Cache\\Notice.json", text4, null);
                    }
                    Settings.Set("HintNotice", num3, null);
                }
                catch (Exception ex5)
                {
                    Logger.Warn(ex5, "下载 PCL 服务器公告失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    FileUtils.Delete(ModBase.PathTemp + "Cache\\Notice.json", false);
                    return;
                }
                Loader.Progress = 0.9;
                try
                {
                    try
                    {
                        foreach (JToken jtoken in text4.Replace("{UNIQUE}", ModBase.Identify).DeserializeJson())
                        {
                            JObject jobject = (JObject)jtoken;
                            int num5 = (int)Math.Round(ModBase.Val(jobject["id"] ?? 0));
                            if (num5 > num2)
                            {
                                bool flag = true;
                                try
                                {
                                    foreach (JToken jtoken2 in (jobject["requirements"] ?? new byte[0]))
                                    {
                                        JProperty jproperty = (JProperty)jtoken2;
                                        if (!ModSecret.ServerRequirement(jproperty.Name, jproperty.Value))
                                        {
                                            flag = false;
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<JToken> enumerator2;
                                    if (enumerator2 != null)
                                    {
                                        enumerator2.Dispose();
                                    }
                                }
                                if (flag)
                                {
                                    int num6 = (int)(jobject["importantLevel"] ?? 2);
                                    bool flag2 = (bool)(jobject["isUpdate"] ?? false);
                                    Logger.Info(string.Format("重要等级 {0}，更新公告 {1}", num6, flag2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                                    if (Settings.Get<int>(flag2 ? "SystemSystemUpdate" : "SystemSystemActivity", null) <= num6 && (!flag2 || !ModSecret.IsUpdating))
                                    {
                                        string text5 = (jobject["title"] ?? "公告").ToString();
                                        string text6 = (jobject["description"] ?? "").ToString().Replace("\\n", "\r\n");
                                        JArray jarray = (JArray)(jobject["buttons"] ?? new JArray());
                                        int count = jarray.Count;
                                        object obj = new object[0];
                                        if (count > 0)
                                        {
                                            Logger.Info(string.Format("显示公告 {0}：{1}", num5, text6), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                                            int num7;
                                            switch (count)
                                            {
                                            case 1:
                                                num7 = ModMain.MyMsgBox(text6, text5, (jarray[0]["text"] ?? "确定").ToString(), "", "", false, true, true, null, null, null);
                                                break;
                                            case 2:
                                                num7 = ModMain.MyMsgBox(text6, text5, (jarray[0]["text"] ?? "确定").ToString(), (jarray[1]["text"] ?? "确定").ToString(), "", false, true, false, null, null, null);
                                                break;
                                            case 3:
                                                num7 = ModMain.MyMsgBox(text6, text5, (jarray[0]["text"] ?? "确定").ToString(), (jarray[1]["text"] ?? "确定").ToString(), (jarray[2]["text"] ?? "确定").ToString(), false, true, false, null, null, null);
                                                break;
                                            default:
                                                Logger.Warn(string.Format("公告 {0} 的弹窗有 {1} 个按钮，无法显示", num5, count), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                                                continue;
                                            }
                                            num7 = num7.Clamp(1, count);
                                            obj = jarray[num7 - 1]["actions"] ?? new byte[0];
                                        }
                                        else
                                        {
                                            obj = jobject["actions"] ?? new byte[0];
                                        }
                                        try
                                        {
                                            foreach (object obj2 in ((IEnumerable)obj))
                                            {
                                                JProperty jproperty2 = (JProperty)obj2;
                                                try
                                                {
                                                    string name = jproperty2.Name;
                                                    if (Operators.CompareString(name, "copy", false) != 0)
                                                    {
                                                        if (Operators.CompareString(name, "website", false) != 0)
                                                        {
                                                            if (Operators.CompareString(name, "stop", false) != 0)
                                                            {
                                                                if (Operators.CompareString(name, "setup", false) != 0)
                                                                {
                                                                    if (Operators.CompareString(name, "update", false) != 0 && Operators.CompareString(name, "slientupdate", false) != 0)
                                                                    {
                                                                        throw new Exception("未知的行动支：" + jproperty2.Name + ", " + jproperty2.Value.ToString());
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JObject jobject2 = (JObject)jproperty2.Value;
                                                                    try
                                                                    {
                                                                        foreach (KeyValuePair<string, JToken> keyValuePair in jobject2)
                                                                        {
                                                                            Settings.Set(keyValuePair.Key, keyValuePair.Value.ToString(), null);
                                                                        }
                                                                        continue;
                                                                    }
                                                                    finally
                                                                    {
                                                                        IEnumerator<KeyValuePair<string, JToken>> enumerator4;
                                                                        if (enumerator4 != null)
                                                                        {
                                                                            enumerator4.Dispose();
                                                                        }
                                                                    }
                                                                }
                                                                JObject jobject3 = (JObject)jproperty2.Value;
                                                                bool flag3 = Operators.CompareString(jproperty2.Name, "slientupdate", false) == 0;
                                                                if (!ModSecret.HasUpdatePermission)
                                                                {
                                                                    if (!flag3)
                                                                    {
                                                                        ModSecret.InputPotatoCode(true);
                                                                    }
                                                                    if (!ModSecret.HasUpdatePermission)
                                                                    {
                                                                        continue;
                                                                    }
                                                                }
                                                                ModSecret.UpdateStart(flag3, null);
                                                            }
                                                            else
                                                            {
                                                                ModMain.FrmMain.EndProgram(false);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ModBase.OpenWebsite(jproperty2.Value.ToString());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ModBase.ClipboardSet(jproperty2.Value.ToString(), true);
                                                    }
                                                }
                                                catch (Exception ex6)
                                                {
                                                    Logger.Error(ex6, string.Format("执行 PCL 服务器公告动作失败（{0}, {1}）", jproperty2.Name, jproperty2.Value), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator enumerator3;
                                            if (enumerator3 is IDisposable)
                                            {
                                                (enumerator3 as IDisposable).Dispose();
                                            }
                                        }
                                    }
                                }
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
                }
                catch (Exception ex7)
                {
                    Logger.Warn(text4, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    Logger.Error(ex7, string.Format("读取 PCL 服务器公告失败（{0}）", text4.Length), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    FileUtils.Delete(ModBase.PathTemp + "Cache\\Notice.json", false);
                }
            }
        }

        private static bool ServerRequirement(string Name, JToken Value)
        {
            checked
            {
                bool flag;
                try
                {
                    uint num = <PrivateImplementationDetails>.ComputeStringHash(Name);
                    if (num <= 2207725236U)
                    {
                        if (num <= 1143669093U)
                        {
                            if (num != 127006290U)
                            {
                                if (num != 233440242U)
                                {
                                    if (num == 1143669093U)
                                    {
                                        if (Operators.CompareString(Name, "setupboolean =", false) == 0)
                                        {
                                            return Settings.Get<bool>(((JProperty)Value).Name, null) == ((JProperty)Value).Value.ToObject<bool>();
                                        }
                                    }
                                }
                                else if (Operators.CompareString(Name, "unlockedtheme =", false) == 0)
                                {
                                    return ModSecret.ThemeCheckOne((int)Math.Round(ModBase.Val(Value)));
                                }
                            }
                            else if (Operators.CompareString(Name, "versionbranch <=", false) == 0)
                            {
                                return ModBase.Val(0) <= ModBase.Val(Value);
                            }
                        }
                        else if (num <= 1834020325U)
                        {
                            if (num != 1699755989U)
                            {
                                if (num == 1834020325U)
                                {
                                    if (Operators.CompareString(Name, "setupinteger >=", false) == 0)
                                    {
                                        return (double)Settings.Get<int>(((JProperty)Value).Name, null) >= ModBase.Val(((JProperty)Value).Value);
                                    }
                                }
                            }
                            else if (Operators.CompareString(Name, "opencount >=", false) == 0)
                            {
                                return (double)Settings.Get<int>("SystemCount", null) >= ModBase.Val(Value);
                            }
                        }
                        else if (num != 1931416207U)
                        {
                            if (num == 2207725236U)
                            {
                                if (Operators.CompareString(Name, "versionbranch >=", false) == 0)
                                {
                                    return ModBase.Val(0) >= ModBase.Val(Value);
                                }
                            }
                        }
                        else if (Operators.CompareString(Name, "debug =", false) == 0)
                        {
                            return Operators.CompareString(ModBase.ModeDebug.ToString().Lower(), Value.ToString().Lower(), false) == 0;
                        }
                    }
                    else if (num <= 3063258025U)
                    {
                        if (num != 2410018677U)
                        {
                            if (num != 2561636181U)
                            {
                                if (num == 3063258025U)
                                {
                                    if (Operators.CompareString(Name, "unlockedtheme !=", false) == 0)
                                    {
                                        return !ModSecret.ThemeCheckOne((int)Math.Round(ModBase.Val(Value)));
                                    }
                                }
                            }
                            else if (Operators.CompareString(Name, "uniqueaddress =", false) == 0)
                            {
                                return Operators.CompareString(ModBase.Identify, Value.ToString(), false) == 0;
                            }
                        }
                        else if (Operators.CompareString(Name, "d10000 <=", false) == 0)
                        {
                            return (double)ModBase.RandomInteger(1, 10000) <= ModBase.Val(Value);
                        }
                    }
                    else if (num <= 3317911237U)
                    {
                        if (num != 3250506571U)
                        {
                            if (num == 3317911237U)
                            {
                                if (Operators.CompareString(Name, "versioncode <=", false) == 0)
                                {
                                    return 414.0 <= ModBase.Val(Value);
                                }
                            }
                        }
                        else if (Operators.CompareString(Name, "versioncode >=", false) == 0)
                        {
                            return 414.0 >= ModBase.Val(Value);
                        }
                    }
                    else if (num != 3779783387U)
                    {
                        if (num == 3914047723U)
                        {
                            if (Operators.CompareString(Name, "setupinteger <=", false) == 0)
                            {
                                return (double)Settings.Get<int>(((JProperty)Value).Name, null) <= ModBase.Val(((JProperty)Value).Value);
                            }
                        }
                    }
                    else if (Operators.CompareString(Name, "opencount <=", false) == 0)
                    {
                        return (double)Settings.Get<int>("SystemCount", null) <= ModBase.Val(Value);
                    }
                    Logger.Warn(string.Format("未知的条件支：{0}, {1}", Name, Value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    flag = false;
                }
                catch (Exception ex)
                {
                    Logger.Warn(string.Format("判断分支条件失败：{0}, {1}", Name, Value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModSecret.vb");
                    flag = false;
                }
                return flag;
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
                if (ModMain.MyMsgBox(string.Format("请点击【复制命令】，然后将复制的命令粘贴到爱发电私信，获取土豆码。{0}输入土豆码后，就能继续更新啦！", "\r\n"), "验证爱发电赞助", "输入土豆码", "复制命令", "取消", false, true, false, null, (ModSecret._Closure$__.$I62-0 == null) ? (ModSecret._Closure$__.$I62-0 = delegate
                {
                    ModBase.ClipboardSet("土豆 " + ModBase.Identify, false);
                    ModBase.OpenWebsite("https://meloong.com/afd/message/df2dfad2960911e89c5252540025c377?potato");
                }) : ModSecret._Closure$__.$I62-0, null) == 3)
                {
                    return;
                }
                text = ModMain.MyMsgBoxInput("输入土豆码", string.Format("请在【爱发电】向龙腾猫跃发送【土豆 {0}】，然后输入自动回复的土豆码以继续更新。{1}自动回复可能比较慢，发送后等一会儿刷新一下网页应该就有了。", ModBase.Identify, "\r\n"), null, new Collection<Validate>
                {
                    new ValidateNullOrWhiteSpace()
                }, "", "确定", "取消", false);
            }
            else
            {
                text = ModMain.MyMsgBoxInput("输入土豆码", string.Format("请输入你获得的土豆码。{0}你的识别码：{1}", "\r\n", ModBase.Identify), null, new Collection<Validate>
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
                        ModMain.MyMsgBox(string.Format("隐藏主题 极客蓝 已解锁！{0}难道，这就是终点了？还是说……？", "\r\n"), "解锁新隐藏主题！", "确定", "", "", false, true, false, null, null, null);
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
                        if (list.Any<string>((ModSecret._Closure$__.$I62-1 == null) ? (ModSecret._Closure$__.$I62-1 = (string f) => !ModSecret.PotatoFeatures.Contains(f)) : ModSecret._Closure$__.$I62-1))
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
                        if (ModMain.MyMsgBox(ex.Message, "错误", "重新输入", "取消", "", true, true, false, null, null, null) == 1)
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
                        text3 = Conversions.ToString(ModBase.GetTimeMs()) + Conversions.ToString(MyWpfExtension.Computer.Info.AvailablePhysicalMemory);
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

        public const string ClsBaseUrl = "https://ap-shanghai.cls.tencentcs.com/track?topic_id=pcl-remote-1253424809";

        public const bool RestrictMcDownload = true;

        public static ModBase.MyColor Color1 = new ModBase.MyColor(52.0, 61.0, 74.0);

        public static ModBase.MyColor Color2 = new ModBase.MyColor(11.0, 91.0, 203.0);

        public static ModBase.MyColor Color3 = new ModBase.MyColor(19.0, 112.0, 243.0);

        public static ModBase.MyColor Color4 = new ModBase.MyColor(72.0, 144.0, 245.0);

        public static ModBase.MyColor Color5 = new ModBase.MyColor(150.0, 192.0, 249.0);

        public static ModBase.MyColor Color6 = new ModBase.MyColor(213.0, 230.0, 253.0);

        public static ModBase.MyColor Color7 = new ModBase.MyColor(222.0, 236.0, 253.0);

        public static ModBase.MyColor Color8 = new ModBase.MyColor(234.0, 242.0, 254.0);

        public static ModBase.MyColor ColorBg0 = new ModBase.MyColor(150.0, 192.0, 249.0);

        public static ModBase.MyColor ColorBg1 = new ModBase.MyColor(190.0, ModSecret.Color7);

        public static ModBase.MyColor ColorGray1 = new ModBase.MyColor(64.0, 64.0, 64.0);

        public static ModBase.MyColor ColorGray2 = new ModBase.MyColor(115.0, 115.0, 115.0);

        public static ModBase.MyColor ColorGray3 = new ModBase.MyColor(140.0, 140.0, 140.0);

        public static ModBase.MyColor ColorGray4 = new ModBase.MyColor(166.0, 166.0, 166.0);

        public static ModBase.MyColor ColorGray5 = new ModBase.MyColor(204.0, 204.0, 204.0);

        public static ModBase.MyColor ColorGray6 = new ModBase.MyColor(235.0, 235.0, 235.0);

        public static ModBase.MyColor ColorGray7 = new ModBase.MyColor(240.0, 240.0, 240.0);

        public static ModBase.MyColor ColorGray8 = new ModBase.MyColor(245.0, 245.0, 245.0);

        public static ModBase.MyColor ColorSemiTransparent = new ModBase.MyColor(1.0, ModSecret.Color8);

        public static int ThemeNow = -1;

        public static int ColorHue = 210;

        public static int ColorSat = 85;

        public static int ColorLightAdjust = 0;

        public static OneOf<int, int[]> ColorHueTopbarDelta = 0;

        public static int ThemeDontClick = 0;

        private static bool IsThemeColorfulLoaded = false;

        private static int ColorHueDelta = 1;

        private static bool IsUpdating = false;

        internal static bool IsUpdateWaitingRestart = false;

        private static bool IsUpdateRestartWaitingTasks = false;

        private static string UpdateMark = "EmptyMark********************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************";

        public static JObject ServerConfig;

        public static ModLoader.LoaderTask<int, int> ServerLoader = new ModLoader.LoaderTask<int, int>("PCL 配置更新", new Action<ModLoader.LoaderTask<int, int>>(ModSecret.ServerSub), null, ThreadPriority.BelowNormal)
        {
            ReloadTimeout = 3600000
        };

        private static List<string> _PotatoFeatures = null;

        private const string EcdsaPublicKey = "RUNTMSAAAAC4QTUNAewh23Q4Q6koHkyIrDIIZUSbua23sf2DiZmIRwSzadISDRyTVTbuWniH3KR7rKj8XBsabms1be6i3c+S";

        private static bool $STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate;

        private static StaticLocalInitFlag $STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init;

        private static bool $STATIC$UpdateStart$0212151280E91E$IsSilentUpdate;
    }
}
