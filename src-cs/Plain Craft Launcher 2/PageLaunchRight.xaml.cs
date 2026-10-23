using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLaunchRight : MyPageRight, IRefreshable, IDispatcherUnhandledException
    {
        public PageLaunchRight()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Init();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Refresh();
            };
            this.RefreshLock = RuntimeHelpers.GetObjectValue(new object());
            this.OnlineLoader = new ModLoader.LoaderTask<ValueTuple<string, bool>, int>("下载主页", new Action<ModLoader.LoaderTask<ValueTuple<string, bool>, int>>(this.OnlineLoaderSub), null, ThreadPriority.Normal)
            {
                ReloadTimeout = 600000
            };
            this.LoadedContentHash = null;
            this.LoadContentLock = RuntimeHelpers.GetObjectValue(new object());
            this.InitializeComponent();
        }

        private void Init()
        {
            this.PanBack.ScrollToHome();
            this.PanLog.Visibility = ModBase.ModeDebug.ToVisibility();
            this.PanHint.Visibility = (ModSecret.CurrentRank < DonationRank.Rank23 && !Settings.Get<bool>("HintSnapshot", null)).ToVisibility();
        }

        private void Refresh()
        {
            ModBase.RunInThread(delegate
            {
                try
                {
                    object refreshLock = this.RefreshLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(refreshLock);
                    lock (refreshLock)
                    {
                        this.RefreshReal();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "加载 PCL 主页自定义信息失败", ModBase.ModeDebug ? LogBehavior.Alert : LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                }
            });
        }

        private void RefreshReal()
        {
            string Content = null;
            string text = null;
            switch (Settings.Get<int>("UiCustomType", null))
            {
            case 1:
                Logger.Info("主页自定义数据来源：本地文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                Content = FileUtils.TryReadAsString(Paths.Base + "PCL\\Custom.xaml", null, null);
                break;
            case 2:
                text = Settings.Get<string>("UiCustomNet", null);
                break;
            case 3:
                switch (Settings.Get<int>("UiCustomPreset", null))
                {
                case 0:
                    Logger.Info("主页预设：你知道吗", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    Content = "\r\n                            <local:MyCard Title=\"你知道吗？\" Margin=\"0,0,0,15\">\r\n                                <TextBlock Margin=\"25,38,23,15\" FontSize=\"13.5\" IsHitTestVisible=\"False\" Text=\"{hint}\" TextWrapping=\"Wrap\" Foreground=\"{DynamicResource ColorBrush1}\" />\r\n                                <local:MyIconButton Height=\"22\" Width=\"22\" Margin=\"9\" VerticalAlignment=\"Top\" HorizontalAlignment=\"Right\" \r\n                                    EventType=\"刷新主页\" EventData=\"/\"\r\n                                    Logo=\"M512.0 838.3c-80.2 0-153.4-29.3-210.2-77.4l75.5-75.5c11.5-11.5 25.8-22.0 25.8-37.0a27.2 27.2 0 0 0-27.1-27.1H104.0c-27.1 0-27.1 23.9-27.1 27.1v271.9a27.1 27.1 0 0 0 27.1 27.1c15.0 0 27.8-16.6 42.5-31.2l77.9-77.9c76.6 67.7 177.1 108.9 287.4 108.9 221.7 0 404.5-166.0 431.2-380.6h-109.8c-25.9 154.2-159.7 271.9-321.3 271.9zM919.9 76.6c-15.0 0-27.8 16.6-42.5 31.3L799.5 185.8c-76.5-67.7-177.1-108.9-287.4-108.9-221.8 0-404.5 166.1-431.3 380.6H190.6c25.9-154.2 159.7-271.9 321.4-271.9 80.2 0 153.4 29.3 210.1 77.4l-75.5 75.5c-11.6 11.5-25.8 22.0-25.8 37.1a27.2 27.2 0 0 0 27.1 27.1h271.9c27.1 0 27.1-23.9 27.1-27.1V103.8a27.1 27.1 0 0 0-27.1-27.1z\" />\r\n                            </local:MyCard>";
                    break;
                case 1:
                    Logger.Info("主页预设：回声洞", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    Content = "\r\n                            <local:MyCard Title=\"回声洞\" Margin=\"0,0,0,15\">\r\n                                <TextBlock Margin=\"25,38,23,15\" FontSize=\"13.5\" IsHitTestVisible=\"False\" Text=\"{cave}\" TextWrapping=\"Wrap\" Foreground=\"{DynamicResource ColorBrush1}\" />\r\n                                <local:MyIconButton Height=\"22\" Width=\"22\" Margin=\"9\" VerticalAlignment=\"Top\" HorizontalAlignment=\"Right\" \r\n                                    EventType=\"刷新主页\" EventData=\"/\"\r\n                                    Logo=\"M512.0 838.3c-80.2 0-153.4-29.3-210.2-77.4l75.5-75.5c11.5-11.5 25.8-22.0 25.8-37.0a27.2 27.2 0 0 0-27.1-27.1H104.0c-27.1 0-27.1 23.9-27.1 27.1v271.9a27.1 27.1 0 0 0 27.1 27.1c15.0 0 27.8-16.6 42.5-31.2l77.9-77.9c76.6 67.7 177.1 108.9 287.4 108.9 221.7 0 404.5-166.0 431.2-380.6h-109.8c-25.9 154.2-159.7 271.9-321.3 271.9zM919.9 76.6c-15.0 0-27.8 16.6-42.5 31.3L799.5 185.8c-76.5-67.7-177.1-108.9-287.4-108.9-221.8 0-404.5 166.1-431.3 380.6H190.6c25.9-154.2 159.7-271.9 321.4-271.9 80.2 0 153.4 29.3 210.1 77.4l-75.5 75.5c-11.6 11.5-25.8 22.0-25.8 37.1a27.2 27.2 0 0 0 27.1 27.1h271.9c27.1 0 27.1-23.9 27.1-27.1V103.8a27.1 27.1 0 0 0-27.1-27.1z\" />\r\n                            </local:MyCard>";
                    break;
                case 2:
                    Logger.Info("主页预设：Minecraft 新闻", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://mcnews.meloong.com";
                    break;
                case 3:
                    Logger.Info("主页预设：简单主页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/MFn233/Custom.xaml";
                    break;
                case 4:
                    Logger.Info("主页预设：每日整合包推荐", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pclsub.sodamc.com/";
                    break;
                case 5:
                    Logger.Info("主页预设：Minecraft 皮肤推荐", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://forgepixel.com/pcl_sub_file";
                    break;
                case 6:
                    Logger.Info("主页预设：OpenBMCLAPI 仪表盘 Lite", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pcl-bmcl.milu.ink/";
                    break;
                case 7:
                    Logger.Info("主页预设：主页市场", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pclhomeplazaoss.lingyunawa.top:26994/p/Homepages/Homepage.Market/Custom.xaml";
                    break;
                case 8:
                    Logger.Info("主页预设：更新日志", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/Joker2184/UpdateHomepage.xaml";
                    break;
                case 9:
                    Logger.Info("主页预设：PCL 新功能说明书", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://raw.gitcode.com/WForst-Breeze/WhatsNewPCL/raw/main/Custom.xaml";
                    break;
                case 10:
                    Logger.Info("主页预设：OpenMCIM Dashboard", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://files.mcimirror.top/PCL";
                    break;
                case 11:
                    Logger.Info("主页预设：杂志主页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://gh-proxy.com/https://github.com/Neclyon/Magazine-Homepage-PCL/raw/main/output/Custom.xaml";
                    break;
                case 12:
                    Logger.Info("主页预设：PCL GitHub 仪表盘", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://ddf.pcl-community.top/Custom.xaml";
                    break;
                case 13:
                    Logger.Info("主页预设：PCL 更新摘要", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://raw.gitcode.com/ENC_Euphony/PCL-AI-Summary-HomePage/raw/master/Custom.xaml";
                    break;
                case 14:
                    Logger.Info("主页预设：今日新闻热点", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pcl.wyc-w.top/index.xaml";
                    break;
                case 15:
                    Logger.Info("主页预设：Minecraft 芝士站", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://mkss-services.awaeric.xyz";
                    break;
                case 16:
                    Logger.Info("主页预设：整合包推荐引擎", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://qawsedrftgyhujiko.fun/pcl2/Custom.xaml";
                    break;
                case 17:
                    Logger.Info("主页预设：Bangumi 番剧主页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://bangumi.p.kaphia.top";
                    break;
                case 18:
                    Logger.Info("主页预设：Bilibili 热门", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://bilibili.p.kaphia.top";
                    break;
                case 19:
                    Logger.Info("主页预设：Music 云音乐热门", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://cloudmusic.p.kaphia.top";
                    break;
                case 20:
                    Logger.Info("主页预设：模组推荐", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://g-fish.dpdns.org/download/ModPage.xaml";
                    break;
                case 21:
                    Logger.Info("主页预设：每日游戏推荐", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://pcl.xn--9iq209asnop94b.top/download/GamePage.xaml";
                    break;
                case 22:
                    Logger.Info("主页预设：Wallpaper 每日壁纸", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://wallpaper.p.kaphia.top";
                    break;
                case 23:
                    Logger.Info("主页预设：Weather 天气预报", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://weather-p.kaphia.top/{variable:PCLDailyWeatherPage:choose}.xaml";
                    break;
                case 24:
                    Logger.Info("主页预设：新一代狗屁王大杂烩主页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    text = "https://end.g-fish.dpdns.org/Custom.xaml";
                    break;
                }
                break;
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = ModMain.ArgumentReplace(text, new Func<string, string>(WebUtility.HtmlEncode), true);
                if (Operators.CompareString(text, Settings.Get<string>("CacheSavedPageUrl", null), false) != 0 || !FileUtils.Exists(ModBase.PathTemp + "Cache\\Custom.xaml"))
                {
                    Logger.Info("主页自定义数据来源：联网全新下载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    ModMain.Hint("正在加载主页……", ModMain.HintType.Blue, true);
                    ModBase.RunInUiWait(delegate
                    {
                        this.LoadContent(null);
                    });
                    Settings.Set("CacheSavedPageVersion", "", null);
                    this.OnlineLoader.Start(new ValueTuple<string, bool>(text, true), false);
                    return;
                }
                Logger.Info("主页自定义数据来源：联网缓存文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                Content = FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\\Custom.xaml", null, null);
                this.OnlineLoader.Start(new ValueTuple<string, bool>(text, false), false);
            }
            ModBase.RunInUi(delegate
            {
                this.LoadContent(Content);
            }, false);
        }

        private void OnlineLoaderSub([TupleElementNames(new string[] { "Address", "ShouldRefresh" })] ModLoader.LoaderTask<ValueTuple<string, bool>, int> Task)
        {
            string item = Task.Input.Item1;
            bool item2 = Task.Input.Item2;
            try
            {
                string text;
                if (item.Contains(".xaml"))
                {
                    text = item.Replace(".xaml", ".xaml.ini");
                }
                else
                {
                    text = item.BeforeFirst("?", false);
                    if (!text.EndsWithF("/", false))
                    {
                        text += "/";
                    }
                    text += "version";
                    if (item.Contains("?"))
                    {
                        text = text + "?" + item.AfterFirst("?", false);
                    }
                }
                string text2 = "";
                try
                {
                    text2 = ModNet.NetRequestByClientRetry(text, null, null, null, null, "*/*", null, false, null, false);
                    if (text2.Length > 1000)
                    {
                        throw new Exception(string.Format("获取的主页版本过长（{0} 字符）", text2.Length));
                    }
                    string text3 = Settings.Get<string>("CacheSavedPageVersion", null);
                    if (Operators.CompareString(text2, "", false) != 0 && Operators.CompareString(text3, "", false) != 0 && Operators.CompareString(text2, text3, false) == 0)
                    {
                        Logger.Info(string.Format("当前缓存的主页已为最新，当前版本：{0}，检查源：{1}", text2, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                        return;
                    }
                    Logger.Info(string.Format("需要下载联网主页，当前版本：{0}，检查源：{1}", text2, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "联网获取主页版本失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                    Logger.Info(string.Format("无法检查联网主页版本，将直接下载，检查源：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                }
                string text4 = ModNet.NetRequestByClientRetry(item, null, null, null, null, "*/*", null, false, null, false);
                Logger.Info(string.Format("已联网下载主页，内容长度：{0}，来源：{1}", text4.Length, item), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                Settings.Set("CacheSavedPageUrl", item, null);
                Settings.Set("CacheSavedPageVersion", text2, null);
                FileUtils.Write(ModBase.PathTemp + "Cache\\Custom.xaml", text4, null);
                ulong? loadedContentHash = this.LoadedContentHash;
                ulong stableHashCode = text4.GetStableHashCode();
                bool? flag = ((loadedContentHash != null) ? new bool?(loadedContentHash.GetValueOrDefault() != stableHashCode) : null);
                if ((flag == null || flag.GetValueOrDefault()) && item2 && flag != null)
                {
                    this.Refresh();
                }
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, string.Format("下载主页失败（{0}）", item), ModBase.ModeDebug ? LogBehavior.Alert : LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
            }
        }

        public void ForceRefresh()
        {
            Logger.Info("要求强制刷新主页", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
            this.ClearCache();
            if (ModMain.FrmMain.PageCurrent.Page == FormMain.PageType.Launch)
            {
                this.PanBack.ScrollToHome();
                this.Refresh();
                return;
            }
            ModMain.FrmMain.PageChange(FormMain.PageType.Launch, FormMain.PageSubType.Default);
        }

        private void ClearCache()
        {
            this.LoadedContentHash = null;
            this.OnlineLoader.Input = new ValueTuple<string, bool>("", true);
            Settings.Set("CacheSavedPageUrl", "", null);
            Settings.Set("CacheSavedPageVersion", "", null);
            Logger.Info("已清空主页缓存", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
        }

        private void LoadContent(string Content)
        {
            try
            {
                object loadContentLock = this.LoadContentLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(loadContentLock);
                lock (loadContentLock)
                {
                    ulong stableHashCode = (Content ?? "").GetStableHashCode();
                    ulong num = stableHashCode;
                    ulong? loadedContentHash = this.LoadedContentHash;
                    if (!((loadedContentHash != null) ? new bool?(num == loadedContentHash.GetValueOrDefault()) : null).GetValueOrDefault())
                    {
                        this.LoadedContentHash = new ulong?(stableHashCode);
                        this.PanCustom.Children.Clear();
                        if (string.IsNullOrWhiteSpace(Content))
                        {
                            Logger.Info("实例化：清空主页 UI，来源为空", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            Content = ModMain.ArgumentReplace(Content, new Func<string, string>(StringUtils.XmlEscape), true);
                            while (Content.Contains("xmlns"))
                            {
                                Content = Content.RegexReplace("xmlns[^\"']*(\"|')[^\"']*(\"|')", "", RegexOptions.None).Replace("xmlns", "");
                            }
                            Content = "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:sys=\"clr-namespace:System;assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:PCL;assembly=Plain Craft Launcher 2\" xmlns:core=\"clr-namespace:MeloongCore;assembly=MeloongCore\" xmlns:corewpf=\"clr-namespace:MeloongCore.Wpf;assembly=MeloongCore.Wpf\">" + Content + "</StackPanel>";
                            Logger.Info(string.Format("实例化：加载主页 UI 开始，最终内容长度：{0}", Content.Count<char>()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                            this.PanCustom.Children.Add((UIElement)ModBase.GetObjectFromXML(Content));
                            int milliseconds = (DateTime.Now - now).Milliseconds;
                            Logger.Info(string.Format("实例化：加载主页 UI 完成，耗时 {0}ms", milliseconds), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                            if (milliseconds > 3000)
                            {
                                ModMain.Hint(string.Format("主页加载过于缓慢（花费了 {0} 秒），请向主页作者反馈此问题，或暂时停止使用该主页", Math.Round((double)milliseconds / 1000.0, 1)), ModMain.HintType.Blue, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("加载失败的主页内容：{0}{1}", "\r\n", Content), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                this.OnLoadContentFailed(ex);
            }
        }

        private void OnLoadContentFailed(Exception ex)
        {
            if (ModBase.ModeDebug || Settings.Get<int>("UiCustomType", null) == 1)
            {
                Logger.Warn(ex, "加载主页失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
                if (ModMain.MyMsgBox((ex is UnauthorizedAccessException) ? ex.Message : string.Format("主页内容编写有误，请根据下列错误信息进行检查：{0}{1}", "\r\n", ex.GetDisplay(false)), "加载主页失败", "重试", "取消", "", false, true, false, null, null, null) == 1)
                {
                    this.ForceRefresh();
                    return;
                }
            }
            else
            {
                Logger.Error(ex, "加载主页失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchRight.xaml.vb");
            }
        }

        void IDispatcherUnhandledException.DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is XamlParseException)
            {
                e.Handled = true;
                this.LoadContent(null);
                this.OnLoadContentFailed(e.Exception);
            }
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint PanHint
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanCustom
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard PanLog
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox LabLog
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private object RefreshLock;

        [TupleElementNames(new string[] { "Address", "ShouldRefresh" })]
        private ModLoader.LoaderTask<ValueTuple<string, bool>, int> OnlineLoader;

        private ulong? LoadedContentHash;

        private object LoadContentLock;
    }
}
