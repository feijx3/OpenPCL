using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCL.My;
using PCLCS;

namespace PCL
{
    [StandardModule]
    public sealed class ModLaunch
    {
        public static bool McLaunchStart(ModLaunch.McLaunchOptions Options = null)
        {
            ModLaunch.CurrentLaunchOptions = Options ?? new ModLaunch.McLaunchOptions();
            if (!ModBase.RunInUi())
            {
                throw new Exception("McLaunchStart 必须在 UI 线程调用！");
            }
            bool flag;
            if (ModLaunch.McLaunchLoader.State == ModBase.LoadState.Loading)
            {
                ModMain.Hint("已有游戏正在启动中！", ModMain.HintType.Red, true);
                flag = false;
            }
            else
            {
                if (ModLaunch.CurrentLaunchOptions.Instance != null && ModMinecraft.McInstanceSelected != ModLaunch.CurrentLaunchOptions.Instance)
                {
                    ModLaunch.McLaunchLog("在启动前切换到版本 " + ModLaunch.CurrentLaunchOptions.Instance.Name, LogLevel.Info);
                    try
                    {
                        ModLaunch.CurrentLaunchOptions.Instance.GetJsonPath();
                    }
                    catch (Exception ex)
                    {
                        ModMain.Hint(string.Format("无法启动 {0}：当前 Minecraft 文件夹中尚未安装该版本！", ModLaunch.CurrentLaunchOptions.Instance.Name), ModMain.HintType.Red, true);
                        return false;
                    }
                    ModLaunch.CurrentLaunchOptions.Instance.Load();
                    if (ModLaunch.CurrentLaunchOptions.Instance.State == ModMinecraft.McInstance.McInstanceState.Error)
                    {
                        ModMain.Hint(string.Format("无法启动 {0}：{1}", ModLaunch.CurrentLaunchOptions.Instance.Name, ModLaunch.CurrentLaunchOptions.Instance.Info), ModMain.HintType.Red, true);
                        return false;
                    }
                    ModMinecraft.McInstanceSelected = ModLaunch.CurrentLaunchOptions.Instance;
                    ModMain.FrmLaunchLeft.RefreshButtonsUI();
                    ModMain.FrmLaunchLeft.RefreshPage(false, false);
                }
                if (ModMinecraft.McInstanceSelected != null && ModMinecraft.McInstanceLock.IsLocked(ModMinecraft.McInstanceSelected))
                {
                    ModMain.Hint(string.Format("版本 {0} 正在被修改，请稍后再启动！", ModMinecraft.McInstanceSelected.Name), ModMain.HintType.Red, true);
                    flag = false;
                }
                else
                {
                    ModMain.FrmMain.AprilGiveup();
                    ModMain.FrmMain.PageStack = ModMain.FrmMain.PageStack.Where<FormMain.PageStackData>((ModLaunch._Closure$__.$I3-0 == null) ? (ModLaunch._Closure$__.$I3-0 = (FormMain.PageStackData p) => p.Page != FormMain.PageType.InstanceSelect) : ModLaunch._Closure$__.$I3-0).ToList<FormMain.PageStackData>();
                    ModLaunch.McLaunchLoader.Start(Options, true);
                    flag = true;
                }
            }
            return flag;
        }

        public static void McLaunchLog(string Text, LogLevel Level = LogLevel.Info)
        {
            Text = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(Text, '*'), '*');
            if (ModBase.ModeDebug)
            {
                ModBase.RunInUi(delegate
                {
                    ModMain.FrmLaunchRight.LabLog.AppendText(string.Format("[{0:HH':'mm':'ss'.'fff}] {1}{2}", DateTime.Now, Text, "\r\n"));
                }, false);
            }
            Logger.Log(Text, Level, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
        }

        private static void McLaunchState(ModLoader.LoaderTask<ModLaunch.McLaunchOptions, object> Loader)
        {
            switch (ModLaunch.McLaunchLoader.State)
            {
            case ModBase.LoadState.Waiting:
            case ModBase.LoadState.Finished:
            case ModBase.LoadState.Failed:
            case ModBase.LoadState.Canceled:
                ModMain.FrmLaunchLeft.PageChangeToLogin();
                return;
            case ModBase.LoadState.Loading:
                ModMain.FrmLaunchRight.LabLog.Text = "";
                return;
            default:
                return;
            }
        }

        private static void McLaunchStart(ModLoader.LoaderTask<ModLaunch.McLaunchOptions, object> Loader)
        {
            ModBase.RunInUiWait(new Action(ModMain.FrmLaunchLeft.PageChangeToLaunching));
            try
            {
                ModLaunch.McLaunchPrecheck();
                ModLaunch.McLaunchLog("预检测已通过", LogLevel.Info);
            }
            catch (Exception ex)
            {
                if (!ex.IsCanceled())
                {
                    ModMain.Hint(ex.Message, ModMain.HintType.Red, true);
                }
                throw;
            }
            ModLaunch.McLaunchOptions currentLaunchOptions = ModLaunch.CurrentLaunchOptions;
            bool flag = ((currentLaunchOptions != null) ? currentLaunchOptions.SaveBatch : null) != null;
            try
            {
                List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>
                {
                    new ModLoader.LoaderTask<int, int>("获取 Java", new Action<ModLoader.LoaderTask<int, int>>(ModLaunch.McLaunchJava), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 4.0,
                        Block = false
                    },
                    ModLaunch.McLoginLoader,
                    new ModLoader.LoaderCombo<string>("补全文件", ModDownload.DlClientFix(ModMinecraft.McInstanceSelected, false, true))
                    {
                        ProgressWeight = 15.0,
                        Show = false
                    },
                    new ModLoader.LoaderTask<string, int>("获取启动参数", new Action<ModLoader.LoaderTask<string, int>>(ModLaunch.McLaunchArgumentMain), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 2.0
                    },
                    new ModLoader.LoaderTask<int, int>("解压文件", new Action<ModLoader.LoaderTask<int, int>>(ModLaunch.McLaunchNatives), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 2.0
                    },
                    new ModLoader.LoaderTask<int, int>("预启动处理", (ModLaunch._Closure$__.$IR11-2 == null) ? (ModLaunch._Closure$__.$IR11-2 = delegate(ModLoader.LoaderTask<int, int> a0)
                    {
                        ModLaunch.McLaunchPrerun();
                    }) : ModLaunch._Closure$__.$IR11-2, null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 1.0
                    },
                    new ModLoader.LoaderTask<int, int>("执行自定义命令", new Action<ModLoader.LoaderTask<int, int>>(ModLaunch.McLaunchCustom), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 1.0
                    },
                    new ModLoader.LoaderTask<int, Process>("启动进程", new Action<ModLoader.LoaderTask<int, Process>>(ModLaunch.McLaunchRun), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 2.0
                    },
                    new ModLoader.LoaderTask<Process, int>("等待游戏窗口出现", new Action<ModLoader.LoaderTask<Process, int>>(ModLaunch.McLaunchWait), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 1.0
                    },
                    new ModLoader.LoaderTask<int, int>("结束处理", (ModLaunch._Closure$__.$IR11-3 == null) ? (ModLaunch._Closure$__.$IR11-3 = delegate(ModLoader.LoaderTask<int, int> a0)
                    {
                        ModLaunch.McLaunchEnd();
                    }) : ModLaunch._Closure$__.$IR11-3, null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 1.0
                    }
                };
                switch (Settings.Get<int>("VersionRamOptimize", ModMinecraft.McInstanceSelected))
                {
                case 0:
                    if (Settings.Get<bool>("LaunchArgumentRam", null))
                    {
                        ((ModLoader.LoaderCombo<string>)list[2]).Block = false;
                        list.Insert(3, new ModLoader.LoaderTask<int, int>("内存优化", new Action<ModLoader.LoaderTask<int, int>>(ModLaunch.McLaunchMemoryOptimize), null, ThreadPriority.Normal)
                        {
                            ProgressWeight = 30.0
                        });
                    }
                    break;
                case 1:
                    ((ModLoader.LoaderCombo<string>)list[2]).Block = false;
                    list.Insert(3, new ModLoader.LoaderTask<int, int>("内存优化", new Action<ModLoader.LoaderTask<int, int>>(ModLaunch.McLaunchMemoryOptimize), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 30.0
                    });
                    break;
                }
                ModLoader.LoaderCombo<object> loaderCombo = new ModLoader.LoaderCombo<object>("Minecraft 启动", list)
                {
                    Show = false
                };
                if (ModLaunch.McLoginLoader.State == ModBase.LoadState.Finished)
                {
                    ModLaunch.McLoginLoader.State = ModBase.LoadState.Waiting;
                }
                ModLaunch.McLaunchLoaderReal = loaderCombo;
                ModLaunch.CanceledHint = null;
                loaderCombo.Start(null, false);
                ModLoader.LoaderTaskbarAdd<object>(loaderCombo);
                while (loaderCombo.State == ModBase.LoadState.Loading)
                {
                    ModMain.FrmLaunchLeft.Dispatcher.Invoke(new Action(ModMain.FrmLaunchLeft.LaunchingRefresh));
                    Thread.Sleep(100);
                }
                ModMain.FrmLaunchLeft.Dispatcher.Invoke(new Action(ModMain.FrmLaunchLeft.LaunchingRefresh));
                switch (loaderCombo.State)
                {
                case ModBase.LoadState.Finished:
                    ModMain.Hint(ModMinecraft.McInstanceSelected.Name + " 启动成功！", ModMain.HintType.Green, true);
                    if (!flag)
                    {
                        ModMain.Telemetry("Minecraft 启动成功", new string[]
                        {
                            "Version",
                            ModMinecraft.McInstanceSelected.VersionDisplayName(),
                            "LoginType",
                            Settings.Get<ModLaunch.McLoginType>("LoginType", null).ToString()
                        });
                    }
                    break;
                case ModBase.LoadState.Failed:
                    throw loaderCombo.Error;
                case ModBase.LoadState.Canceled:
                    if (ModLaunch.CanceledHint == null)
                    {
                        ModMain.Hint(flag ? "已取消导出启动脚本！" : "已取消启动！", ModMain.HintType.Blue, true);
                    }
                    else
                    {
                        ModMain.Hint(ModLaunch.CanceledHint, ModMain.HintType.Green, true);
                    }
                    break;
                default:
                    throw new Exception("错误的状态改变：" + loaderCombo.State.ToString());
                }
            }
            catch (Exception ex2)
            {
                ex2.ThrowIfCanceled();
                Exception ex3 = ex2;
                while (!ex3.Message.StartsWithF("$", false))
                {
                    if (ex3.InnerException == null)
                    {
                        ModLaunch.McLaunchLog("错误：" + ex2.GetDisplay(true), LogLevel.Info);
                        Logger.Error(ex2, flag ? "导出启动脚本失败" : "Minecraft 启动失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                        if (!flag)
                        {
                            ModMain.Telemetry("Minecraft 启动失败", new string[]
                            {
                                "Version",
                                ModMinecraft.McInstanceSelected.VersionDisplayName(),
                                "LoginType",
                                Settings.Get<ModLaunch.McLoginType>("LoginType", null).ToString(),
                                "Exception",
                                ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(ex2.GetDisplay(true), '*'), '*')
                            });
                        }
                        throw;
                    }
                    ex3 = ex3.InnerException;
                }
                ModMain.MyMsgBox(ex3.Message.TrimStart(new char[] { '$' }), flag ? "导出启动脚本失败" : "启动失败", "确定", "", "", false, true, false, null, null, null);
                throw;
            }
        }

        private static void McLaunchMemoryOptimize(ModLoader.LoaderTask<int, int> Loader)
        {
            ModLaunch._Closure$__12-0 CS$<>8__locals1 = new ModLaunch._Closure$__12-0(CS$<>8__locals1);
            ModLaunch.McLaunchLog("内存优化开始", LogLevel.Info);
            CS$<>8__locals1.$VB$Local_Finished = false;
            ModBase.RunInNewThread(delegate
            {
                PageOtherTest.MemoryOptimize(false);
                CS$<>8__locals1.$VB$Local_Finished = true;
            }, "Launch Memory Optimize", ThreadPriority.Normal);
            while (!CS$<>8__locals1.$VB$Local_Finished && !Loader.IsCanceled)
            {
                if (Loader.Progress < 0.7)
                {
                    Loader.Progress += 0.007;
                }
                else
                {
                    Loader.Progress += (0.95 - Loader.Progress) * 0.02;
                }
                Thread.Sleep(100);
            }
        }

        private static void McLaunchPrecheck()
        {
            if (Settings.Get<bool>("SystemDebugDelay", null))
            {
                Thread.Sleep(ModBase.RandomInteger(100, 2000));
            }
            if (ModMinecraft.McInstanceSelected.PathIndie.Contains("!") || ModMinecraft.McInstanceSelected.PathIndie.Contains(";"))
            {
                throw new Exception("游戏路径中不可包含 ! 或 ;（" + ModMinecraft.McInstanceSelected.PathIndie + "）");
            }
            if (ModMinecraft.McInstanceSelected.PathVersion.Contains("!") || ModMinecraft.McInstanceSelected.PathVersion.Contains(";"))
            {
                throw new Exception("游戏路径中不可包含 ! 或 ;（" + ModMinecraft.McInstanceSelected.PathVersion + "）");
            }
            if (ModMinecraft.McInstanceSelected == null)
            {
                throw new Exception("未选择 Minecraft 版本！");
            }
            ModMinecraft.McInstanceSelected.Load();
            if (ModMinecraft.McInstanceSelected.State == ModMinecraft.McInstance.McInstanceState.Error)
            {
                throw new Exception("Minecraft 存在问题：" + ModMinecraft.McInstanceSelected.Info);
            }
            string CheckResult = "";
            ModBase.RunInUiWait(delegate
            {
                CheckResult = ModLaunch.McLoginAble(ModLaunch.McLoginInput());
            });
            if (Operators.CompareString(CheckResult, "", false) != 0)
            {
                throw new ArgumentException(CheckResult);
            }
            ModLaunch.McLaunchOptions currentLaunchOptions = ModLaunch.CurrentLaunchOptions;
            if (((currentLaunchOptions != null) ? currentLaunchOptions.SaveBatch : null) == null && !Settings.Get<bool>("HintBuy", null) && Settings.Get<ModLaunch.McLoginType>("LoginType", null) != ModLaunch.McLoginType.Ms)
            {
                if (CultureInfo.CurrentCulture.Name.StartsWithF("zh", false) || CultureInfo.CurrentUICulture.Name.StartsWithF("zh", false))
                {
                    ModBase.RunInNewThread((ModLaunch._Closure$__.$I13-2 == null) ? (ModLaunch._Closure$__.$I13-2 = delegate
                    {
                        int num2 = Settings.Get<int>("SystemLaunchCount", null);
                        if (num2 <= 280)
                        {
                            if (num2 <= 70)
                            {
                                if (num2 <= 15)
                                {
                                    if (num2 != 3 && num2 != 8 && num2 != 15)
                                    {
                                        return;
                                    }
                                }
                                else if (num2 != 30 && num2 != 50 && num2 != 70)
                                {
                                    return;
                                }
                            }
                            else if (num2 <= 130)
                            {
                                if (num2 != 90 && num2 != 110 && num2 != 130)
                                {
                                    return;
                                }
                            }
                            else if (num2 != 180 && num2 != 220 && num2 != 280)
                            {
                                return;
                            }
                        }
                        else if (num2 <= 750)
                        {
                            if (num2 <= 450)
                            {
                                if (num2 != 330 && num2 != 380 && num2 != 450)
                                {
                                    return;
                                }
                            }
                            else if (num2 != 550 && num2 != 660 && num2 != 750)
                            {
                                return;
                            }
                        }
                        else if (num2 <= 1100)
                        {
                            if (num2 != 880 && num2 != 950 && num2 != 1100)
                            {
                                return;
                            }
                        }
                        else if (num2 <= 1500)
                        {
                            if (num2 != 1300 && num2 != 1500)
                            {
                                return;
                            }
                        }
                        else if (num2 != 1700 && num2 != 1900)
                        {
                            return;
                        }
                        if (ModMain.MyMsgBox("你已经启动了 " + Conversions.ToString(Settings.Get<int>("SystemLaunchCount", null)) + " 次 Minecraft 啦！\r\n如果觉得 Minecraft 还不错，可以购买正版支持一下，毕竟开发游戏也真的很不容易……不要一直白嫖啦。\r\n\r\n在登录一次正版账号后，就不会再出现这个提示了！", "考虑一下正版？", "支持正版游戏！", "下次一定", "", false, true, false, null, null, null) == 1)
                        {
                            ModBase.OpenWebsite("https://www.xbox.com/zh-cn/games/store/minecraft-java-bedrock-edition-for-pc/9nxp44l49shj");
                        }
                    }) : ModLaunch._Closure$__.$I13-2, "Buy Minecraft", ThreadPriority.Normal);
                    return;
                }
                if (Settings.Get<ModLaunch.McLoginType>("LoginType", null) == ModLaunch.McLoginType.Legacy)
                {
                    int num = ModMain.MyMsgBox("你必须先登录正版账号，才能进行离线登录！", "正版验证", "购买正版", "试玩", "返回", false, true, false, (ModLaunch._Closure$__.$I13-3 == null) ? (ModLaunch._Closure$__.$I13-3 = delegate
                    {
                        ModBase.OpenWebsite("https://www.xbox.com/zh-cn/games/store/minecraft-java-bedrock-edition-for-pc/9nxp44l49shj");
                    }) : ModLaunch._Closure$__.$I13-3, null, null);
                    if (num == 2)
                    {
                        ModMain.Hint("游戏将以试玩模式启动！", ModMain.HintType.Red, true);
                        ModLaunch.CurrentLaunchOptions.ExtraGameArgs.Add("--demo");
                        return;
                    }
                    if (num != 3)
                    {
                        return;
                    }
                    throw new OperationCanceledException();
                }
            }
        }

        public static string McLoginName()
        {
            switch (Settings.Get<ModLaunch.McLoginType>("LoginType", null))
            {
            case ModLaunch.McLoginType.Legacy:
                if (Operators.CompareString(Settings.Get<string>("LoginLegacyName", null), "", false) != 0)
                {
                    return Settings.Get<string>("LoginLegacyName", null).ToString().BeforeFirst("\u00a8", false);
                }
                break;
            case ModLaunch.McLoginType.Nide:
                if (Operators.CompareString(Settings.Get<string>("CacheNideName", null), "", false) != 0)
                {
                    return Settings.Get<string>("CacheNideName", null);
                }
                break;
            case ModLaunch.McLoginType.Auth:
                if (Operators.CompareString(Settings.Get<string>("CacheAuthName", null), "", false) != 0)
                {
                    return Settings.Get<string>("CacheAuthName", null);
                }
                break;
            case ModLaunch.McLoginType.Ms:
                if (Operators.CompareString(Settings.Get<string>("CacheMsV2Name", null), "", false) != 0)
                {
                    return Settings.Get<string>("CacheMsV2Name", null);
                }
                break;
            }
            string text;
            if (Operators.CompareString(Settings.Get<string>("CacheMsV2Name", null), "", false) != 0)
            {
                text = Settings.Get<string>("CacheMsV2Name", null);
            }
            else if (Operators.CompareString(Settings.Get<string>("CacheNideName", null), "", false) != 0)
            {
                text = Settings.Get<string>("CacheNideName", null);
            }
            else if (Operators.CompareString(Settings.Get<string>("CacheAuthName", null), "", false) != 0)
            {
                text = Settings.Get<string>("CacheAuthName", null);
            }
            else if (Operators.CompareString(Settings.Get<string>("LoginLegacyName", null), "", false) != 0)
            {
                text = Settings.Get<string>("LoginLegacyName", null).ToString().BeforeFirst("\u00a8", false);
            }
            else
            {
                text = null;
            }
            return text;
        }

        public static string McLoginAble()
        {
            switch (Settings.Get<ModLaunch.McLoginType>("LoginType", null))
            {
            case ModLaunch.McLoginType.Legacy:
                return ModMain.FrmLoginLegacy.IsVaild();
            case ModLaunch.McLoginType.Nide:
                if (Operators.CompareString(Settings.Get<string>("CacheNideAccess", null), "", false) == 0)
                {
                    return ModMain.FrmLoginNide.IsVaild();
                }
                return "";
            case ModLaunch.McLoginType.Auth:
                if (Operators.CompareString(Settings.Get<string>("CacheAuthAccess", null), "", false) == 0)
                {
                    return ModMain.FrmLoginAuth.IsVaild();
                }
                return "";
            case ModLaunch.McLoginType.Ms:
                if (Operators.CompareString(Settings.Get<string>("CacheMsV2OAuthRefresh", null), "", false) == 0)
                {
                    return ModMain.FrmLoginMs.IsVaild();
                }
                return "";
            }
            return "未知的登录方式";
        }

        public static string McLoginAble(ModLaunch.McLoginData LoginData)
        {
            switch (LoginData.Type)
            {
            case ModLaunch.McLoginType.Legacy:
                return PageLoginLegacy.IsVaild((ModLaunch.McLoginLegacy)LoginData);
            case ModLaunch.McLoginType.Nide:
                return PageLoginNide.IsVaild((ModLaunch.McLoginServer)LoginData);
            case ModLaunch.McLoginType.Auth:
                return PageLoginAuth.IsVaild((ModLaunch.McLoginServer)LoginData);
            case ModLaunch.McLoginType.Ms:
                return PageLoginMs.IsVaild((ModLaunch.McLoginMs)LoginData);
            }
            return "未知的登录方式";
        }

        public static ModLaunch.McLoginData McLoginInput()
        {
            ModLaunch.McLoginData mcLoginData = null;
            ModLaunch.McLoginType mcLoginType = Settings.Get<ModLaunch.McLoginType>("LoginType", null);
            ModLaunch.McLoginData mcLoginData2;
            try
            {
                switch (mcLoginType)
                {
                case ModLaunch.McLoginType.Legacy:
                    mcLoginData = PageLoginLegacy.GetLoginData();
                    break;
                case ModLaunch.McLoginType.Nide:
                    if (Operators.CompareString(Settings.Get<string>("CacheNideAccess", null), "", false) == 0)
                    {
                        mcLoginData = PageLoginNide.GetLoginData();
                    }
                    else
                    {
                        mcLoginData = PageLoginNideSkin.GetLoginData();
                    }
                    break;
                case ModLaunch.McLoginType.Auth:
                    if (Operators.CompareString(Settings.Get<string>("CacheAuthAccess", null), "", false) == 0)
                    {
                        mcLoginData = PageLoginAuth.GetLoginData();
                    }
                    else
                    {
                        mcLoginData = PageLoginAuthSkin.GetLoginData();
                    }
                    break;
                case ModLaunch.McLoginType.Ms:
                    if (Operators.CompareString(Settings.Get<string>("CacheMsV2OAuthRefresh", null), "", false) == 0)
                    {
                        mcLoginData = PageLoginMs.GetLoginData();
                    }
                    else
                    {
                        mcLoginData = PageLoginMsSkin.GetLoginData();
                    }
                    break;
                }
                mcLoginData2 = mcLoginData;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取登录输入信息失败（{0}）", mcLoginType), ex);
            }
            return mcLoginData2;
        }

        private static void McLoginStart(ModLoader.LoaderTask<ModLaunch.McLoginData, ModLaunch.McLoginResult> Data)
        {
            ModLaunch.McLaunchLog("登录加载已开始", LogLevel.Info);
            string text = ModLaunch.McLoginAble(Data.Input);
            if (Operators.CompareString(text, "", false) != 0)
            {
                throw new ArgumentException(text);
            }
            ModLoader.LoaderBase loaderBase = null;
            switch (Data.Input.Type)
            {
            case ModLaunch.McLoginType.Legacy:
                loaderBase = ModLaunch.McLoginLegacyLoader;
                break;
            case ModLaunch.McLoginType.Nide:
                loaderBase = ModLaunch.McLoginNideLoader;
                break;
            case ModLaunch.McLoginType.Auth:
                loaderBase = ModLaunch.McLoginAuthLoader;
                break;
            case ModLaunch.McLoginType.Ms:
                loaderBase = ModLaunch.McLoginMsLoader;
                break;
            }
            loaderBase.WaitForExit(Data.Input, ModLaunch.McLoginLoader, Data.IsForceRestarting, default(CancellationToken), null);
            object obj = NewLateBinding.LateGet(loaderBase, null, "Output", new object[0], null, null, null);
            Data.Output = ((obj != null) ? ((ModLaunch.McLoginResult)obj) : default(ModLaunch.McLoginResult));
            ModBase.RunInUi((ModLaunch._Closure$__.$I25-0 == null) ? (ModLaunch._Closure$__.$I25-0 = delegate
            {
                ModMain.FrmLaunchLeft.RefreshPage(true, false);
            }) : ModLaunch._Closure$__.$I25-0, false);
            ModLaunch.McLaunchLog("登录加载已结束", LogLevel.Info);
        }

        private static void McLoginMsStart(ModLoader.LoaderTask<ModLaunch.McLoginMs, ModLaunch.McLoginResult> Data)
        {
            ModLaunch.McLoginMs input = Data.Input;
            string userName = input.UserName;
            ModLaunch.McLaunchLog("登录方式：正版（" + ((Operators.CompareString(userName, "", false) == 0) ? "尚未登录" : userName) + "）", LogLevel.Info);
            Data.Progress = 0.05;
            long num = Settings.Get<long>("CacheMsV2Expires", null);
            if (!Data.IsForceRestarting && num > 0L && num > ModBase.ToUnixTimestampUtc(DateTime.UtcNow) && Operators.CompareString(input.UserName, Settings.Get<string>("CacheMsV2Name", null), false) == 0)
            {
                Data.Output = new ModLaunch.McLoginResult
                {
                    Name = input.UserName,
                    Type = "Microsoft",
                    AccessToken = Settings.Get<string>("CacheMsV2Access", null),
                    Uuid = Settings.Get<string>("CacheMsV2Uuid", null),
                    ClientToken = Settings.Get<string>("CacheMsV2Uuid", null),
                    ProfileJson = Settings.Get<string>("CacheMsV2ProfileJson", null)
                };
                ModLaunch.McLaunchLog("无需登录，AccessToken 尚未过期", LogLevel.Info);
            }
            else
            {
                ValueTuple<string, string> valueTuple;
                if (Operators.CompareString(input.OAuthRefreshToken, "", false) != 0)
                {
                    valueTuple = ModLaunch.MsLoginStep1Refresh(input.OAuthRefreshToken);
                    if (Operators.CompareString(valueTuple.Item1, "Relogin", false) != 0)
                    {
                        goto IL_014F;
                    }
                }
                valueTuple = ModLaunch.MsLoginStep1New(Data);
                IL_014F:
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                Data.Progress = 0.25;
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                string item = valueTuple.Item1;
                string item2 = valueTuple.Item2;
                string text = ModLaunch.MsLoginStep2(item);
                Data.Progress = 0.4;
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                ValueTuple<string, string> valueTuple2 = ModLaunch.MsLoginStep3(text);
                Data.Progress = 0.55;
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                ValueTuple<string, int> valueTuple3 = ModLaunch.MsLoginStep4(valueTuple2);
                num = (long)valueTuple3.Item2;
                Data.Progress = 0.7;
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                ModLaunch.MsLoginStep5(valueTuple3.Item1);
                Data.Progress = 0.85;
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                ValueTuple<string, string, string> valueTuple4 = ModLaunch.MsLoginStep6(valueTuple3.Item1);
                Data.Progress = 0.98;
                Settings.Set("CacheMsV2OAuthRefresh", item2, null);
                Settings.Set("CacheMsV2Access", valueTuple3.Item1, null);
                Settings.Set("CacheMsV2Uuid", valueTuple4.Item1, null);
                Settings.Set("CacheMsV2Name", valueTuple4.Item2, null);
                Settings.Set("CacheMsV2ProfileJson", valueTuple4.Item3, null);
                Settings.Set("CacheMsV2Expires", valueTuple3.Item2, null);
                JObject jobject = (JObject)Settings.Get<string>("LoginMsJson", null).DeserializeJson();
                jobject.Remove(input.UserName);
                jobject[valueTuple4.Item2] = item2;
                Settings.Set("LoginMsJson", jobject.ToString(0, new JsonConverter[0]), null);
                Data.Output = new ModLaunch.McLoginResult
                {
                    AccessToken = valueTuple3.Item1,
                    Name = valueTuple4.Item2,
                    Uuid = valueTuple4.Item1,
                    Type = "Microsoft",
                    ClientToken = valueTuple4.Item1,
                    ProfileJson = valueTuple4.Item3
                };
            }
            ModLaunch.McLaunchLog(string.Format("微软登录结束，AccessToken 将在 {0} 过期", DateTimeOffset.FromUnixTimeSeconds(num).LocalDateTime), LogLevel.Info);
            Settings.Set("HintBuy", true, null);
            if (ModSecret.ThemeUnlock(10, false, null))
            {
                ModMain.MyMsgBox("感谢你对正版游戏的支持！\r\n隐藏主题 跳票红 已解锁！", "提示", "确定", "", "", false, true, false, null, null, null);
            }
        }

        private static void McLoginServerStart(ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult> Data)
        {
            ModLaunch.McLoginServer input = Data.Input;
            bool flag = false;
            bool flag2 = false;
            ModLaunch.McLaunchLog("登录方式：" + input.Description, LogLevel.Info);
            Data.Progress = 0.05;
            if (Data.Input.ForceReselectProfile || Operators.CompareString(Settings.Get<string>("Cache" + input.Token + "Username", null), Data.Input.UserName, false) != 0 || Operators.CompareString(Settings.Get<string>("Cache" + input.Token + "Pass", null), Data.Input.Password, false) != 0 || Operators.CompareString(Settings.Get<string>("Cache" + input.Token + "Access", null), "", false) == 0 || Operators.CompareString(Settings.Get<string>("Cache" + input.Token + "Client", null), "", false) == 0 || Operators.CompareString(Settings.Get<string>("Cache" + input.Token + "Uuid", null), "", false) == 0 || Operators.CompareString(Settings.Get<string>("Cache" + input.Token + "Name", null), "", false) == 0)
            {
                goto IL_021C;
            }
            try
            {
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                ModLaunch.McLoginRequestValidate(ref Data);
                goto IL_027B;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "验证登录失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                if (ex.IsBadNetwork())
                {
                    throw new Exception("$登录失败：你的网络环境不佳，导致难以连接到海外服务器。\r\n请稍后再试，或使用加速器、VPN 改善网络环境。");
                }
            }
            Data.Progress = 0.25;
            IL_01AC:
            try
            {
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                ModLaunch.McLoginRequestRefresh(ref Data, flag);
                goto IL_027B;
            }
            catch (Exception ex2)
            {
                ModLaunch.McLaunchLog("刷新登录失败：" + ex2.GetDisplay(true), LogLevel.Info);
                if (flag2)
                {
                    throw new Exception("二轮刷新登录失败", ex2);
                }
            }
            Data.Progress = (flag ? 0.85 : 0.45);
            IL_021C:
            try
            {
                if (Data.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
                flag = ModLaunch.McLoginRequestLogin(ref Data);
            }
            catch (Exception ex3)
            {
                ModLaunch.McLaunchLog("登录失败：" + ex3.GetDisplay(true), LogLevel.Info);
                throw;
            }
            if (flag)
            {
                ModLaunch.McLaunchLog("重新进行刷新登录", LogLevel.Info);
                flag2 = true;
                Data.Progress = 0.65;
                goto IL_01AC;
            }
            IL_027B:
            Data.Progress = 0.95;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            checked
            {
                try
                {
                    if (Operators.CompareString(Settings.Get<string>("Login" + input.Token + "Email", null), "", false) != 0)
                    {
                        list.AddRange(Settings.Get<string>("Login" + input.Token + "Email", null).Split("\u00a8", false));
                    }
                    if (Operators.CompareString(Settings.Get<string>("Login" + input.Token + "Pass", null), "", false) != 0)
                    {
                        list2.AddRange(Settings.Get<string>("Login" + input.Token + "Pass", null).Split("\u00a8", false));
                    }
                    int num = list.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        dictionary.Add(list[i], list2[i]);
                    }
                    dictionary.Remove(input.UserName);
                    list = new List<string>(dictionary.Keys);
                    list.Insert(0, input.UserName);
                    list2 = new List<string>(dictionary.Values);
                    list2.Insert(0, input.Password);
                    Settings.Set("Login" + input.Token + "Email", list.Join('\u00a8'), null);
                    Settings.Set("Login" + input.Token + "Pass", list2.Join('\u00a8'), null);
                }
                catch (Exception ex4)
                {
                    Logger.Error(ex4, "保存启动记录失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    Settings.Set("Login" + input.Token + "Email", "", null);
                    Settings.Set("Login" + input.Token + "Pass", "", null);
                }
            }
        }

        private static void McLoginLegacyStart(ModLoader.LoaderTask<ModLaunch.McLoginLegacy, ModLaunch.McLoginResult> Data)
        {
            ModLaunch.McLoginLegacy input = Data.Input;
            ModLaunch.McLaunchLog("登录方式：离线（" + input.UserName + "）", LogLevel.Info);
            Data.Progress = 0.1;
            Data.Output.Name = input.UserName;
            Data.Output.Uuid = ModLaunch.McLoginLegacyUuidWithCustomSkin(input.UserName, input.SkinType, input.SkinName);
            Data.Output.Type = "Legacy";
            Data.Output.AccessToken = Data.Output.Uuid;
            Data.Output.ClientToken = Data.Output.Uuid;
            List<string> list = new List<string>();
            if (Operators.CompareString(Settings.Get<string>("LoginLegacyName", null), "", false) != 0)
            {
                list.AddRange(Settings.Get<string>("LoginLegacyName", null).ToString().Split("\u00a8", false));
            }
            list.Remove(input.UserName);
            list.Insert(0, input.UserName);
            Settings.Set("LoginLegacyName", list.Join('\u00a8'), null);
        }

        private static void McLoginRequestValidate(ref ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult> Data)
        {
            ModLaunch.McLaunchLog("验证登录开始（Validate, " + Data.Input.Token + "）", LogLevel.Info);
            string text = Settings.Get<string>("Cache" + Data.Input.Token + "Access", null);
            string text2 = Settings.Get<string>("Cache" + Data.Input.Token + "Client", null);
            string text3 = Settings.Get<string>("Cache" + Data.Input.Token + "Uuid", null);
            string text4 = Settings.Get<string>("Cache" + Data.Input.Token + "Name", null);
            JObject jobject = new JObject(new object[]
            {
                new JProperty("accessToken", text),
                new JProperty("clientToken", text2),
                new JProperty("requestUser", true)
            });
            string text5 = Data.Input.BaseUrl + "/validate";
            HttpMethod post = HttpMethod.Post;
            string text6 = null;
            object obj = jobject.ToString(0, new JsonConverter[0]);
            string text7 = "application/json; charset=utf-8";
            string text8 = "*/*";
            string[,] array = new string[1, 2];
            array[0, 0] = "Accept-Language";
            array[0, 1] = "zh-CN";
            ModNet.NetRequestByClientRetry(text5, post, text6, obj, text7, text8, array, false, null, false);
            Data.Output.AccessToken = text;
            Data.Output.ClientToken = text2;
            Data.Output.Uuid = text3;
            Data.Output.Name = text4;
            Data.Output.Type = Data.Input.Token;
            ModLaunch.McLaunchLog("验证登录成功（Validate, " + Data.Input.Token + "）", LogLevel.Info);
        }

        private static void McLoginRequestRefresh(ref ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult> Data, bool RequestUser)
        {
            ModLaunch.McLaunchLog("刷新登录开始（Refresh, " + Data.Input.Token + "）", LogLevel.Info);
            string text = Data.Input.BaseUrl + "/refresh";
            HttpMethod post = HttpMethod.Post;
            string text2 = null;
            object obj = new JObject(new object[]
            {
                new JProperty("selectedProfile", new JObject(new object[]
                {
                    new JProperty("name", Settings.Get<string>(string.Format("Cache{0}Name", Data.Input.Token), null)),
                    new JProperty("id", Settings.Get<string>(string.Format("Cache{0}Uuid", Data.Input.Token), null))
                })),
                new JProperty("accessToken", Settings.Get<string>(string.Format("Cache{0}Access", Data.Input.Token), null)),
                new JProperty("requestUser", true)
            }).ToString(0, new JsonConverter[0]);
            string text3 = "application/json; charset=utf-8";
            string text4 = "*/*";
            string[,] array = new string[1, 2];
            array[0, 0] = "Accept-Language";
            array[0, 1] = "zh-CN";
            JObject jobject = (JObject)ModNet.NetRequestByClientRetry(text, post, text2, obj, text3, text4, array, true, null, false).DeserializeJson();
            if (jobject["selectedProfile"] == null)
            {
                throw new Exception("选择的角色 " + Settings.Get<string>("Cache" + Data.Input.Token + "Name", null) + " 无效！");
            }
            Data.Output.AccessToken = jobject["accessToken"].ToString();
            Data.Output.ClientToken = jobject["clientToken"].ToString();
            Data.Output.Uuid = jobject["selectedProfile"]["id"].ToString();
            Data.Output.Name = jobject["selectedProfile"]["name"].ToString();
            Data.Output.Type = Data.Input.Token;
            Settings.Set("Cache" + Data.Input.Token + "Access", Data.Output.AccessToken, null);
            Settings.Set("Cache" + Data.Input.Token + "Client", Data.Output.ClientToken, null);
            Settings.Set("Cache" + Data.Input.Token + "Uuid", Data.Output.Uuid, null);
            Settings.Set("Cache" + Data.Input.Token + "Name", Data.Output.Name, null);
            Settings.Set("Cache" + Data.Input.Token + "Username", Data.Input.UserName, null);
            Settings.Set("Cache" + Data.Input.Token + "Pass", Data.Input.Password, null);
            ModLaunch.McLaunchLog("刷新登录成功（Refresh, " + Data.Input.Token + "）", LogLevel.Info);
        }

        private static bool McLoginRequestLogin(ref ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult> Data)
        {
            bool flag2;
            try
            {
                ModLaunch._Closure$__35-0 CS$<>8__locals1 = new ModLaunch._Closure$__35-0(CS$<>8__locals1);
                bool flag = false;
                ModLaunch.McLaunchLog("登录开始（Login, " + Data.Input.Token + "）", LogLevel.Info);
                JObject jobject = new JObject(new object[]
                {
                    new JProperty("agent", new JObject(new object[]
                    {
                        new JProperty("name", "Minecraft"),
                        new JProperty("version", 1)
                    })),
                    new JProperty("username", Data.Input.UserName),
                    new JProperty("password", Data.Input.Password),
                    new JProperty("requestUser", true)
                });
                ModLaunch._Closure$__35-0 CS$<>8__locals2 = CS$<>8__locals1;
                string text = Data.Input.BaseUrl + "/authenticate";
                HttpMethod post = HttpMethod.Post;
                string text2 = null;
                object obj = jobject.ToString(0, new JsonConverter[0]);
                string text3 = "application/json; charset=utf-8";
                string text4 = "*/*";
                string[,] array = new string[1, 2];
                array[0, 0] = "Accept-Language";
                array[0, 1] = "zh-CN";
                CS$<>8__locals2.$VB$Local_LoginJson = (JObject)ModNet.NetRequestByClientRetry(text, post, text2, obj, text3, text4, array, true, null, false).DeserializeJson();
                if (CS$<>8__locals1.$VB$Local_LoginJson["availableProfiles"].Count<JToken>() == 0)
                {
                    if (Data.Input.ForceReselectProfile)
                    {
                        ModMain.Hint("你还没有创建角色，无法更换！", ModMain.HintType.Red, true);
                    }
                    throw new Exception("$你还没有创建角色，请在创建角色后再试！");
                }
                if (Data.Input.ForceReselectProfile && CS$<>8__locals1.$VB$Local_LoginJson["availableProfiles"].IsSingle<JToken>())
                {
                    ModMain.Hint("你的账户中只有一个角色，无法更换！", ModMain.HintType.Red, true);
                }
                CS$<>8__locals1.$VB$Local_SelectedName = null;
                CS$<>8__locals1.$VB$Local_SelectedId = null;
                if ((CS$<>8__locals1.$VB$Local_LoginJson["selectedProfile"] == null || Data.Input.ForceReselectProfile) && CS$<>8__locals1.$VB$Local_LoginJson["availableProfiles"].Count<JToken>() > 1)
                {
                    flag = true;
                    string text5 = Settings.Get<string>("Cache" + Data.Input.Token + "Uuid", null);
                    try
                    {
                        foreach (JToken jtoken in CS$<>8__locals1.$VB$Local_LoginJson["availableProfiles"])
                        {
                            if (Operators.CompareString(jtoken["id"].ToString(), text5, false) == 0)
                            {
                                CS$<>8__locals1.$VB$Local_SelectedName = jtoken["name"].ToString();
                                CS$<>8__locals1.$VB$Local_SelectedId = jtoken["id"].ToString();
                                ModLaunch.McLaunchLog("根据缓存选择的角色：" + CS$<>8__locals1.$VB$Local_SelectedName, LogLevel.Info);
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
                    if (CS$<>8__locals1.$VB$Local_SelectedName == null)
                    {
                        ModLaunch.McLaunchLog("要求玩家选择角色", LogLevel.Info);
                        ModBase.RunInUiWait(delegate
                        {
                            List<IMyRadio> list = new List<IMyRadio>();
                            List<JToken> list2 = new List<JToken>();
                            try
                            {
                                foreach (JToken jtoken2 in CS$<>8__locals1.$VB$Local_LoginJson["availableProfiles"])
                                {
                                    list.Add(new MyRadioBox
                                    {
                                        Text = jtoken2["name"].ToString()
                                    });
                                    list2.Add(jtoken2);
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
                            int value = ModMain.MyMsgBoxSelect(list, "选择使用的角色", "确定", "", false).Value;
                            CS$<>8__locals1.$VB$Local_SelectedName = list2[value]["name"].ToString();
                            CS$<>8__locals1.$VB$Local_SelectedId = list2[value]["id"].ToString();
                        });
                        ModLaunch.McLaunchLog("玩家选择的角色：" + CS$<>8__locals1.$VB$Local_SelectedName, LogLevel.Info);
                    }
                }
                else
                {
                    CS$<>8__locals1.$VB$Local_SelectedName = CS$<>8__locals1.$VB$Local_LoginJson["selectedProfile"]["name"].ToString();
                    CS$<>8__locals1.$VB$Local_SelectedId = CS$<>8__locals1.$VB$Local_LoginJson["selectedProfile"]["id"].ToString();
                }
                Data.Output.AccessToken = CS$<>8__locals1.$VB$Local_LoginJson["accessToken"].ToString();
                Data.Output.ClientToken = CS$<>8__locals1.$VB$Local_LoginJson["clientToken"].ToString();
                Data.Output.Name = CS$<>8__locals1.$VB$Local_SelectedName;
                Data.Output.Uuid = CS$<>8__locals1.$VB$Local_SelectedId;
                Data.Output.Type = Data.Input.Token;
                Settings.Set("Cache" + Data.Input.Token + "Access", Data.Output.AccessToken, null);
                Settings.Set("Cache" + Data.Input.Token + "Client", Data.Output.ClientToken, null);
                Settings.Set("Cache" + Data.Input.Token + "Uuid", Data.Output.Uuid, null);
                Settings.Set("Cache" + Data.Input.Token + "Name", Data.Output.Name, null);
                Settings.Set("Cache" + Data.Input.Token + "Username", Data.Input.UserName, null);
                Settings.Set("Cache" + Data.Input.Token + "Pass", Data.Input.Password, null);
                ModLaunch.McLaunchLog("登录成功（Login, " + Data.Input.Token + "）", LogLevel.Info);
                flag2 = flag;
            }
            catch (Exception ex)
            {
                string display = ex.GetDisplay(false);
                Logger.Info(ex, "登录失败原始错误信息", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                if (ex is HttpRequestCodeException)
                {
                    string text6 = null;
                    try
                    {
                        string response = ((HttpRequestCodeException)ex).Response;
                        if (response.ContainsIgnoreCase("errorMessage"))
                        {
                            text6 = response.DeserializeJson()["errorMessage"].ToString();
                        }
                    }
                    catch (Exception ex2)
                    {
                    }
                    if (!string.IsNullOrWhiteSpace(text6))
                    {
                        if (text6.Contains("密码错误") || text6.ContainsIgnoreCase("Incorrect username or password"))
                        {
                            ModLaunch.McLaunchLog("密码错误，退出登录", LogLevel.Info);
                            ModLaunch.McLoginType type = Data.Input.Type;
                            if (type != ModLaunch.McLoginType.Nide)
                            {
                                if (type == ModLaunch.McLoginType.Auth)
                                {
                                    ModBase.RunInUi(new Action(PageLoginAuthSkin.ExitLogin), false);
                                }
                            }
                            else
                            {
                                ModBase.RunInUi(new Action(PageLoginNideSkin.ExitLogin), false);
                            }
                        }
                        throw new Exception("$登录失败：" + text6);
                    }
                }
                if (display.Contains("(403)"))
                {
                    ModLaunch.McLoginType type2 = Data.Input.Type;
                    if (type2 == ModLaunch.McLoginType.Nide)
                    {
                        throw new Exception("$登录失败，以下为可能的原因：\r\n - 输入的账号或密码错误。\r\n - 密码错误次数过多，导致被暂时屏蔽。请不要操作，等待 10 分钟后再试。\r\n" + (Data.Input.UserName.Contains("@") ? "" : " - 登录账号应为邮箱或统一通行证账号，而非游戏角色 ID。\r\n") + " - 只注册了账号，但没有加入对应服务器。");
                    }
                    if (type2 == ModLaunch.McLoginType.Auth)
                    {
                        throw new Exception("$登录失败，以下为可能的原因：\r\n - 输入的账号或密码错误。\r\n - 登录尝试过于频繁，导致被暂时屏蔽。请不要操作，等待 10 分钟后再试。\r\n - 只注册了账号，但没有在皮肤站新建角色。");
                    }
                    flag2 = false;
                }
                else
                {
                    if (ex.IsBadNetwork())
                    {
                        throw new Exception("$登录失败：你的网络环境不佳，导致难以连接到海外服务器。\r\n请稍后再试，或使用加速器、VPN 改善网络环境。");
                    }
                    if (ex.Message.StartsWithF("$", false))
                    {
                        throw;
                    }
                    throw new Exception("登录失败：" + ex.Message, ex);
                }
            }
            return flag2;
        }

        [return: TupleElementNames(new string[] { "OAuthAccessToken", "OAuthRefreshToken" })]
        private static ValueTuple<string, string> MsLoginStep1New(ModLoader.LoaderTask<ModLaunch.McLoginMs, ModLaunch.McLoginResult> Data)
        {
            ModMain.MyMsgBoxConverter myMsgBoxConverter;
            do
            {
                ModLaunch.McLaunchLog("开始微软登录步骤 1/6（原始登录）", LogLevel.Info);
                JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode", HttpMethod.Post, null, string.Format("client_id={0}&tenant=/consumers&scope=XboxLive.signin%20offline_access", "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1"), "application/x-www-form-urlencoded", "*/*", null, true, null, false).DeserializeJson();
                ModLaunch.McLaunchLog("网页登录地址：" + jobject["verification_uri"].ToString(), LogLevel.Info);
                myMsgBoxConverter = new ModMain.MyMsgBoxConverter
                {
                    Content = jobject,
                    ForceWait = true,
                    Type = ModMain.MyMsgBoxType.Login
                };
                ModMain.WaitingMyMsgBox.Enqueue(myMsgBoxConverter);
                while (myMsgBoxConverter.Result == null)
                {
                    Thread.Sleep(100);
                }
                if (!(myMsgBoxConverter.Result is ModBase.RestartException))
                {
                    goto IL_015E;
                }
            }
            while (ModMain.MyMsgBox(string.Format("请在登录时选择 {0}其他登录方法{1}，然后选择 {2}使用我的密码{3}。{4}如果没有该选项，请选择 {5}设置密码{6}，设置完毕后再登录。", new object[]
            {
                ModBase.vbLQ,
                ModBase.vbRQ,
                ModBase.vbLQ,
                ModBase.vbRQ,
                "\r\n",
                ModBase.vbLQ,
                ModBase.vbRQ
            }), "需要使用密码登录", "重新登录", "设置密码", "取消", false, true, false, null, (ModLaunch._Closure$__.$I36-0 == null) ? (ModLaunch._Closure$__.$I36-0 = delegate
            {
                ModBase.OpenWebsite("https://account.live.com/password/Change");
            }) : ModLaunch._Closure$__.$I36-0, null) == 1);
            throw new OperationCanceledException();
            IL_015E:
            if (myMsgBoxConverter.Result is Exception)
            {
                throw (Exception)myMsgBoxConverter.Result;
            }
            ValueTuple<string, string> valueTuple = new ValueTuple<string, string>(Conversions.ToString(NewLateBinding.LateIndexGet(myMsgBoxConverter.Result, new object[] { 0 }, null)), Conversions.ToString(NewLateBinding.LateIndexGet(myMsgBoxConverter.Result, new object[] { 1 }, null)));
            return valueTuple;
        }

        [return: TupleElementNames(new string[] { "OAuthAccessToken", "OAuthRefreshToken" })]
        private static ValueTuple<string, string> MsLoginStep1Refresh(string Code)
        {
            ModLaunch.McLaunchLog("开始微软登录步骤 1/6（刷新登录）", LogLevel.Info);
            string text4;
            ValueTuple<string, string> valueTuple;
            try
            {
                string text = "https://login.live.com/oauth20_token.srf";
                HttpMethod post = HttpMethod.Post;
                int num = 2;
                object obj = string.Format("client_id={0}&refresh_token={1}&grant_type=refresh_token&scope=XboxLive.signin%20offline_access", "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1", StringUtils.FormUrlEscape(Code));
                string text2 = "application/x-www-form-urlencoded";
                string text3 = "*/*";
                string[,] array = new string[2, 2];
                array[0, 0] = "Accept-Language";
                array[0, 1] = "en-US,en;q=0.5";
                array[1, 0] = "X-Requested-With";
                array[1, 1] = "XMLHttpRequest";
                text4 = ModNet.NetRequestByClientMultiple(text, post, num, obj, text2, text3, array, false, 25000, null, false);
            }
            catch (HttpRequestCodeException ex)
            {
                string response = ex.Response;
                if (response.ContainsIgnoreCase("must sign in again") || response.ContainsIgnoreCase("password expired") || (response.Contains("refresh_token") && response.ContainsIgnoreCase("is not valid")) || response.ContainsIgnoreCase("expired"))
                {
                    valueTuple = new ValueTuple<string, string>("Relogin", "");
                    return valueTuple;
                }
                if (response.Contains("Account security interrupt"))
                {
                    ModMain.MyMsgBox("该账号由于安全问题无法登陆，请前往微软账户页获取更多信息。", "登录失败", "我知道了", "", "", true, true, false, null, null, null);
                    throw new OperationCanceledException();
                }
                if (response.Contains("service abuse"))
                {
                    ModMain.MyMsgBox("非常抱歉，该账号已被微软封禁，无法登录。", "登录失败", "我知道了", "", "", true, true, false, null, null, null);
                    throw new OperationCanceledException();
                }
                throw;
            }
            JObject jobject = (JObject)text4.DeserializeJson();
            string text5 = jobject["access_token"].ToString();
            string text6 = jobject["refresh_token"].ToString();
            valueTuple = new ValueTuple<string, string>(text5, text6);
            return valueTuple;
        }

        private static string MsLoginStep2(string AccessToken)
        {
            ModLaunch.McLaunchLog("开始微软登录步骤 2/6", LogLevel.Info);
            string text = "{\r\n           \"Properties\": {\r\n               \"AuthMethod\": \"RPS\",\r\n               \"SiteName\": \"user.auth.xboxlive.com\",\r\n               \"RpsTicket\": \"" + (AccessToken.StartsWithF("d=", false) ? "" : "d=") + AccessToken + "\"\r\n           },\r\n           \"RelyingParty\": \"http://auth.xboxlive.com\",\r\n           \"TokenType\": \"JWT\"\r\n        }";
            return ModNet.NetRequestByClientMultiple("https://user.auth.xboxlive.com/user/authenticate", HttpMethod.Post, 3, text, "application/json", "*/*", null, true, 25000, null, false).DeserializeJson()["Token"].ToString();
        }

        [return: TupleElementNames(new string[] { "XSTSToken", "UHS" })]
        private static ValueTuple<string, string> MsLoginStep3(string XBLToken)
        {
            ModLaunch.McLaunchLog("开始微软登录步骤 3/6", LogLevel.Info);
            string text = "{\r\n           \"Properties\": {\r\n               \"SandboxId\": \"RETAIL\",\r\n               \"UserTokens\": [\"" + XBLToken + "\"]\r\n           },\r\n           \"RelyingParty\": \"rp://api.minecraftservices.com/\",\r\n           \"TokenType\": \"JWT\"\r\n        }";
            string text2;
            try
            {
                text2 = ModNet.NetRequestByClientMultiple("https://xsts.auth.xboxlive.com/xsts/authorize", HttpMethod.Post, 3, text, "application/json", "*/*", null, true, 25000, null, false);
            }
            catch (HttpRequestCodeException ex)
            {
                if (ex.Response.Contains("2148916227"))
                {
                    ModMain.MyMsgBox("该账号似乎已被微软封禁，无法登录。", "登录失败", "我知道了", "", "", true, true, false, null, null, null);
                    throw new OperationCanceledException();
                }
                if (ex.Response.Contains("2148916233"))
                {
                    if (ModMain.MyMsgBox("你尚未注册 Xbox 账户，请在注册后再登录。", "登录提示", "注册", "取消", "", false, true, false, null, null, null) == 1)
                    {
                        ModBase.OpenWebsite("https://signup.live.com/signup");
                    }
                    throw new OperationCanceledException();
                }
                if (ex.Response.Contains("2148916235"))
                {
                    ModMain.MyMsgBox(string.Format("你的网络所在的国家或地区无法登录微软账号。{0}请使用加速器或 VPN，然后再试。", "\r\n"), "登录失败", "我知道了", "", "", false, true, false, null, null, null);
                    throw new OperationCanceledException();
                }
                if (ex.Response.Contains("2148916238"))
                {
                    if (ModMain.MyMsgBox("该账号年龄不足，你需要先修改出生日期，然后才能登录。\r\n该账号目前填写的年龄是否在 13 岁以上？", "登录提示", "13 岁以上", "12 岁以下", "我不知道", false, true, false, null, null, null) == 1)
                    {
                        ModBase.OpenWebsite("https://account.live.com/editprof.aspx");
                        ModMain.MyMsgBox("请在打开的网页中修改账号的出生日期（至少改为 18 岁以上）。\r\n在修改成功后等待一分钟，然后再回到 PCL，就可以正常登录了！", "登录提示", "确定", "", "", false, true, false, null, null, null);
                    }
                    else
                    {
                        ModBase.OpenWebsite("https://support.microsoft.com/zh-cn/account-billing/如何更改-microsoft-帐户上的出生日期-837badbc-999e-54d2-2617-d19206b9540a");
                        ModMain.MyMsgBox("请根据打开的网页的说明，修改账号的出生日期（至少改为 18 岁以上）。\r\n在修改成功后等待一分钟，然后再回到 PCL，就可以正常登录了！", "登录提示", "确定", "", "", false, true, false, null, null, null);
                    }
                    throw new OperationCanceledException();
                }
                throw;
            }
            JObject jobject = (JObject)text2.DeserializeJson();
            return new ValueTuple<string, string>(jobject["Token"].ToString(), jobject["DisplayClaims"]["xui"][0]["uhs"].ToString());
        }

        [return: TupleElementNames(new string[] { "AccessToken", "ExpiresAt" })]
        private static ValueTuple<string, int> MsLoginStep4([TupleElementNames(new string[] { "XSTSToken", "UHS" })] ValueTuple<string, string> Tokens)
        {
            ModLaunch.McLaunchLog("开始微软登录步骤 4/6", LogLevel.Info);
            string text = new JObject(new JProperty("identityToken", string.Format("XBL3.0 x={0};{1}", Tokens.Item2, Tokens.Item1))).ToString(0, new JsonConverter[0]);
            string text2;
            try
            {
                text2 = ModNet.NetRequestByClientRetry("https://api.minecraftservices.com/authentication/login_with_xbox", HttpMethod.Post, null, text, "application/json", "*/*", null, true, null, false);
            }
            catch (HttpRequestCodeException ex)
            {
                if (ex.StatusCode == (HttpStatusCode)429)
                {
                    Logger.Warn(ex, "微软登录第 4 步汇报 429", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    throw new Exception("$登录尝试太过频繁，请等待几分钟后再试！");
                }
                if (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    Logger.Warn(ex, "微软登录第 4 步汇报 403", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    throw new Exception("$当前 IP 的登录尝试异常。\r\n如果你使用了 VPN 或加速器，请把它们关掉或更换节点后再试！");
                }
                if (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    Logger.Warn(ex, "微软登录第 4 步汇报 503", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    throw new Exception("$Mojang 服务器出现问题，请稍后再试。\r\n你的网络是正常的，PCL 也是正常的，是 Mojang 出问题了……");
                }
                string response = ex.Response;
                if (response != null && response.Contains("ACCOUNT_SUSPENDED"))
                {
                    ModMain.MyMsgBox("该账号似乎已被微软封禁，无法登录。", "登录失败", "我知道了", "", "", true, true, false, null, null, null);
                    throw new OperationCanceledException();
                }
                throw;
            }
            JObject jobject = (JObject)text2.DeserializeJson();
            checked
            {
                return new ValueTuple<string, int>(jobject["access_token"].ToString(), (int)(unchecked((long)jobject["expires_in"].ToObject<int>()) + ModBase.GetUnixTimestampUtc() - 3600L));
            }
        }

        private static void MsLoginStep5(string AccessToken)
        {
            ModLaunch.McLaunchLog("开始微软登录步骤 5/6", LogLevel.Info);
            string text = "https://api.minecraftservices.com/entitlements/mcstore";
            HttpMethod httpMethod = null;
            int num = 2;
            object obj = null;
            string text2 = "application/json";
            string text3 = "*/*";
            string[,] array = new string[1, 2];
            array[0, 0] = "Authorization";
            array[0, 1] = "Bearer " + AccessToken;
            string text4 = ModNet.NetRequestByClientMultiple(text, httpMethod, num, obj, text2, text3, array, true, 25000, null, false);
            try
            {
                JObject jobject = (JObject)text4.DeserializeJson();
                if (!jobject.ContainsKey("items") || !jobject["items"].Any<JToken>())
                {
                    int num2 = ModMain.MyMsgBox("你尚未购买正版 Minecraft，或者 Xbox Game Pass 已到期。", "登录失败", "购买 Minecraft", "取消", "", false, true, false, null, null, null);
                    if (num2 == 1)
                    {
                        ModBase.OpenWebsite("https://www.xbox.com/zh-cn/games/store/minecraft-java-bedrock-edition-for-pc/9nxp44l49shj");
                    }
                    throw new OperationCanceledException();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("微软登录第 5 步异常：{0}", text4), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                throw;
            }
        }

        [return: TupleElementNames(new string[] { "UUID", "UserName", "ProfileJson" })]
        private static ValueTuple<string, string, string> MsLoginStep6(string AccessToken)
        {
            ModLaunch.McLaunchLog("开始微软登录步骤 6/6", LogLevel.Info);
            string text4;
            try
            {
                string text = "https://api.minecraftservices.com/minecraft/profile";
                HttpMethod httpMethod = null;
                int num = 2;
                object obj = null;
                string text2 = "application/json";
                string text3 = "*/*";
                string[,] array = new string[1, 2];
                array[0, 0] = "Authorization";
                array[0, 1] = "Bearer " + AccessToken;
                text4 = ModNet.NetRequestByClientMultiple(text, httpMethod, num, obj, text2, text3, array, true, 25000, null, false);
            }
            catch (HttpRequestCodeException ex)
            {
                HttpStatusCode statusCode = ex.StatusCode;
                if (statusCode == HttpStatusCode.NotFound)
                {
                    Logger.Warn(ex, "微软登录第 6 步汇报 404", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    ModBase.RunInNewThread((ModLaunch._Closure$__.$I42-0 == null) ? (ModLaunch._Closure$__.$I42-0 = delegate
                    {
                        int num2 = ModMain.MyMsgBox("请先创建 Minecraft 玩家档案，然后再重新登录。", "登录失败", "创建档案", "取消", "", false, true, false, null, null, null);
                        if (num2 == 1)
                        {
                            ModBase.OpenWebsite("https://www.minecraft.net/zh-hans/msaprofile/mygames/editprofile");
                        }
                    }) : ModLaunch._Closure$__.$I42-0, "Login Failed: Create Profile", ThreadPriority.Normal);
                    throw new OperationCanceledException();
                }
                if (statusCode == (HttpStatusCode)429)
                {
                    Logger.Warn(ex, "微软登录第 6 步汇报 429", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    throw new Exception("$登录尝试太过频繁，请等待几分钟后再试！");
                }
                throw;
            }
            JObject jobject = (JObject)text4.DeserializeJson();
            string text5 = jobject["id"].ToString();
            string text6 = jobject["name"].ToString();
            return new ValueTuple<string, string, string>(text5, text6, text4);
        }

        private static string McLoginLegacyUuidWithCustomSkin(string UserName, int SkinType, string SkinName)
        {
            string text = Conversions.ToString(ModLaunch.McLoginLegacyUuid(UserName));
            checked
            {
                switch (SkinType)
                {
                case 0:
                    return text;
                case 1:
                    while (Operators.CompareString(ModMinecraft.McSkinSex(text), "Steve", false) != 0)
                    {
                        if (text.EndsWithF("FFFFF", false))
                        {
                            text = text.Substring(0, 27) + "00000";
                        }
                        text = text.Substring(0, 27) + (long.Parse(text.Substring(27), NumberStyles.AllowHexSpecifier) + 1L).ToString("X").PadLeft(5, '0');
                    }
                    return text;
                case 2:
                    while (Operators.CompareString(ModMinecraft.McSkinSex(text), "Alex", false) != 0)
                    {
                        if (text.EndsWithF("FFFFF", false))
                        {
                            text = text.Substring(0, 27) + "00000";
                        }
                        text = text.Substring(0, 27) + (long.Parse(text.Substring(27), NumberStyles.AllowHexSpecifier) + 1L).ToString("X").PadLeft(5, '0');
                    }
                    return text;
                case 3:
                    try
                    {
                        if (Operators.CompareString(SkinName, "", false) != 0 && ModMinecraft.McInstanceSelected != null && ModMinecraft.McInstanceSelected.Version.Vanilla.Major < 20)
                        {
                            Logger.Info(string.Format("由于离线皮肤设置，使用正版 UUID：{0}", SkinName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                            text = Conversions.ToString(ModLaunch.McLoginMojangUuid(SkinName, false));
                        }
                        return text;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "皮肤信息获取失败，游戏将以无皮肤的方式启动", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                        return text;
                    }
                    break;
                case 4:
                    IL_01E6:
                    if (Operators.CompareString(ModMinecraft.McSkinSex(text), Settings.Get<bool>("LaunchSkinSlim", null) ? "Alex" : "Steve", false) == 0)
                    {
                        return text;
                    }
                    break;
                default:
                    return text;
                }
                if (text.EndsWithF("FFFFF", false))
                {
                    text = text.Substring(0, 27) + "00000";
                }
                text = text.Substring(0, 27) + (long.Parse(text.Substring(27), NumberStyles.AllowHexSpecifier) + 1L).ToString("X").PadLeft(5, '0');
                goto IL_01E6;
            }
        }

        public static object McLoginMojangUuid(string Name, bool ThrowOnNotFound)
        {
            object obj;
            if (Name.Trim().Length == 0)
            {
                obj = new string('0', 32);
            }
            else
            {
                string text = ModBase.ReadIni(ModBase.PathTemp + "Cache\\Uuid\\Mojang.ini", Name, "");
                if (Strings.Len(text) == 32)
                {
                    obj = text;
                }
                else
                {
                    try
                    {
                        JObject jobject = (JObject)ModNet.NetRequestByClientRetry("https://api.mojang.com/users/profiles/minecraft/" + Name, null, null, null, null, "*/*", null, true, null, false).DeserializeJson();
                        if (jobject == null)
                        {
                            throw new FileNotFoundException("正版玩家档案不存在（" + Name + "）");
                        }
                        text = (string)(jobject["id"] ?? "");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("从官网获取正版 Uuid 失败（{0}）", Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                        if (ThrowOnNotFound || Operators.CompareString(ex.GetType().Name, "FileNotFoundException", false) != 0)
                        {
                            throw new Exception("从官网获取正版 Uuid 失败", ex);
                        }
                        text = Conversions.ToString(ModLaunch.McLoginLegacyUuid(Name));
                    }
                    if (Strings.Len(text) != 32)
                    {
                        throw new Exception("获取的正版 Uuid 长度不足（" + text + "）");
                    }
                    ModBase.WriteIni(ModBase.PathTemp + "Cache\\Uuid\\Mojang.ini", Name, text);
                    obj = text;
                }
            }
            return obj;
        }

        public static object McLoginLegacyUuid(string Name)
        {
            string text = Name.Length.ToString("X").EnsureLength('0', 16) + Name.GetStableHashCode().ToString("X").EnsureLength('0', 16);
            return string.Concat(new string[]
            {
                text.Substring(0, 12),
                "3",
                text.Substring(13, 3),
                "9",
                text.Substring(17, 15)
            });
        }

        private static void McLaunchJava(ModLoader.LoaderTask<int, int> Task)
        {
            ModLaunch.McLaunchJavaSelected = ModJava.SelectOrDownloadJava(ModMinecraft.McInstanceSelected, true, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 1.0));
            if (!Task.IsCanceled)
            {
                if (ModLaunch.McLaunchJavaSelected == null)
                {
                    throw new OperationCanceledException();
                }
                if (ModBase.ModeDebug)
                {
                    ModLaunch.McLaunchLog(string.Format("预期 Java 版本范围：{0}", ModJava.GetJavaRequirement(ModMinecraft.McInstanceSelected).Item1), LogLevel.Info);
                    ModLaunch.McLaunchLog(string.Format("Java 版本设置类别：{0}", Settings.Get<int>("VersionArgumentJavaV2", ModMinecraft.McInstanceSelected)), LogLevel.Info);
                }
                ModLaunch.McLaunchLog("选择的 Java：" + ModLaunch.McLaunchJavaSelected.ToString(), LogLevel.Info);
            }
        }

        private static void McLaunchArgumentMain(ModLoader.LoaderTask<string, int> Loader)
        {
            List<string> list = new List<string>();
            ModLaunch.McLaunchLog("参数获取开始", LogLevel.Info);
            list.AddRange(ModLaunch.McLaunchArgumentsJVM());
            list.Add((string)ModMinecraft.McInstanceSelected.JsonObject["mainClass"]);
            list.AddRange(ModLaunch.McLaunchArgumentsGame());
            ModLaunch.McLaunchLog("=================== 启动参数开始 ===================", LogLevel.Info);
            List<ValueTuple<string, string>> list2 = ModLaunch.McLaunchArgumentsReplace(list).ToList<ValueTuple<string, string>>();
            string text = "";
            try
            {
                foreach (string text2 in list)
                {
                    try
                    {
                        foreach (ValueTuple<string, string> valueTuple in list2)
                        {
                            text2 = text2.Replace(valueTuple.Item1, valueTuple.Item2);
                        }
                    }
                    finally
                    {
                        List<ValueTuple<string, string>>.Enumerator enumerator2;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    if (text2.IndexOfAny(new char[] { '&', '|', '<', '>', '^', ' ' }) >= 0 && !text2.StartsWithF("\"", false) && !text2.EndsWithF("\"", false))
                    {
                        text2 = string.Format("\"{0}\"", text2.Replace("\"", "\\\""));
                    }
                    ModLaunch.McLaunchLog(text2, LogLevel.Info);
                    text = text + text2 + " ";
                }
            }
            finally
            {
                List<string>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            ModLaunch.McLaunchLog("=================== 启动参数结束 ===================", LogLevel.Info);
            ModLaunch.McLaunchArgument = text.TrimEnd(new char[0]);
        }

        private static List<string> McLaunchArgumentsJVM()
        {
            ModLaunch._Closure$__51-0 CS$<>8__locals1 = new ModLaunch._Closure$__51-0(CS$<>8__locals1);
            List<string> list = new List<string>();
            if (ModMinecraft.McInstanceSelected.JsonObject["arguments"] != null && ModMinecraft.McInstanceSelected.JsonObject["arguments"]["jvm"] != null)
            {
                ModMinecraft.McInstance mcInstance = ModMinecraft.McInstanceSelected;
                for (;;)
                {
                    ModLaunch.McLaunchLog(string.Format("新版 JVM 参数：从版本 JSON 获取（{0}）", mcInstance.Name), LogLevel.Info);
                    if (mcInstance.JsonObject["arguments"] != null && mcInstance.JsonObject["arguments"]["jvm"] != null)
                    {
                        try
                        {
                            foreach (JToken jtoken in mcInstance.JsonObject["arguments"]["jvm"])
                            {
                                JTokenType type = jtoken.Type;
                                if (type != 1)
                                {
                                    if (type == 8)
                                    {
                                        list.Add(jtoken.ToString().Trim());
                                    }
                                }
                                else
                                {
                                    JObject jobject = (JObject)jtoken;
                                    if ((!jobject.ContainsKey("rules") || ModMinecraft.McJsonRuleCheck(jobject["rules"])) && jobject.ContainsKey("value"))
                                    {
                                        if (jobject["value"].Type == 8)
                                        {
                                            list.Add(jobject["value"].ToString().Trim());
                                        }
                                        else
                                        {
                                            try
                                            {
                                                foreach (JToken jtoken2 in jobject["value"])
                                                {
                                                    list.Add(jtoken2.ToString().Trim());
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
                    if (Operators.CompareString(mcInstance.InheritName, "", false) == 0)
                    {
                        break;
                    }
                    mcInstance = new ModMinecraft.McInstance(mcInstance.InheritName);
                }
                list = list.Select<string, string>((ModLaunch._Closure$__.$I51-0 == null) ? (ModLaunch._Closure$__.$I51-0 = delegate(string a)
                {
                    if (!a.Contains(" ") || a.StartsWithF("\"", false) || a.EndsWithF("\"", false))
                    {
                        return a;
                    }
                    return string.Format("\"{0}\"", a.Replace("\"", "\\\""));
                }) : ModLaunch._Closure$__.$I51-0).ToList<string>();
            }
            else
            {
                ModLaunch.McLaunchLog("旧版 JVM 参数", LogLevel.Info);
                list.Add("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump");
                list.Add("-Djava.library.path=${natives_directory}");
                list.Add("-cp");
                list.Add("${classpath}");
            }
            string text = Settings.Get<string>("VersionAdvanceJvm", ModMinecraft.McInstanceSelected);
            if (Operators.CompareString(text, "", false) == 0)
            {
                text = Settings.Get<string>("LaunchAdvanceJvm", null);
            }
            if (Operators.CompareString(text, "", false) != 0)
            {
                text = ModMain.ArgumentReplace(text, null, true);
            }
            list = ModLaunch.DeduplicateJavaArguments(ModLaunch.SplitJavaArguments(list.Join(' ').Replace("McEmu= ", "McEmu=") + " " + text).ToList<string>(), true);
            if (Operators.CompareString(ModLaunch.McLoginLoader.Output.Type, "Nide", false) == 0)
            {
                list.Add("-javaagent:\"${pure_directory}\\nide8auth.jar\"=\"" + Settings.Get<string>("VersionServerNide", ModMinecraft.McInstanceSelected) + "\"");
            }
            if (Operators.CompareString(ModLaunch.McLoginLoader.Output.Type, "Auth", false) == 0)
            {
                if (ModLaunch.McLaunchJavaSelected.Version.Major >= 6)
                {
                    list.Add("-Djavax.net.ssl.trustStoreType=WINDOWS-ROOT");
                }
                string text2 = Settings.Get<string>("VersionServerAuthServer", ModMinecraft.McInstanceSelected);
                try
                {
                    ModLaunch.McLaunchLog("开始 Authlib Injector Prefetch", LogLevel.Info);
                    string text3 = ModNet.NetRequestByClientRetry(text2, null, null, null, null, "*/*", null, false, Encoding.UTF8, false);
                    list.Add("-javaagent:\"${pure_directory}\\authlib-injector.jar\"=\"" + text2 + "\"");
                    list.Add("-Dauthlibinjector.side=client");
                    list.Add("-Dauthlibinjector.yggdrasil.prefetched=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(text3)));
                }
                catch (Exception ex)
                {
                    throw new Exception("无法连接到第三方登录服务器（" + text2 + "）", ex);
                }
            }
            bool flag = !Settings.Get<bool>("LaunchAdvanceDisableJLW", null) && !Settings.Get<bool>("VersionAdvanceDisableJLW", ModMinecraft.McInstanceSelected) && !ModBase.IsGBKEncoding;
            if (flag && text.Contains("-javaagent"))
            {
                flag = false;
                ModLaunch.McLaunchLog("由于使用了自定义 javaagent，已禁用 JLW，这可能导致游戏崩溃！", LogLevel.Warn);
            }
            if (flag)
            {
                if (ModLaunch.McLaunchJavaSelected.Version.Major >= 9)
                {
                    list.Add("--add-exports");
                    list.Add("cpw.mods.bootstraplauncher/cpw.mods.bootstraplauncher=ALL-UNNAMED");
                }
                list.Add("-Doolloo.jlw.tmpdir=${pure_directory}");
            }
            ModLaunch.McLaunchLog("使用 JLW：" + Conversions.ToString(flag), LogLevel.Info);
            bool flag2 = !Settings.Get<bool>("LaunchAdvanceDisableLUA", null) && !Settings.Get<bool>("VersionAdvanceDisableLUA", ModMinecraft.McInstanceSelected) && ModMinecraft.McLibListGet(ModMinecraft.McInstanceSelected, false).Select<ModMinecraft.McLibEntry, string>((ModLaunch._Closure$__.$I51-1 == null) ? (ModLaunch._Closure$__.$I51-1 = (ModMinecraft.McLibEntry e) => e.OriginalName) : ModLaunch._Closure$__.$I51-1).Contains("org.lwjgl:lwjgl:3.4.1");
            if (flag2)
            {
                list.Add(string.Format("-javaagent:\"{0}\"", LaunchUtils.ExtractPatch("Launch\\LwjglUnsafeAgent.jar", ModBase.PathPure.Value)));
            }
            ModLaunch.McLaunchLog("使用 LUA：" + Conversions.ToString(flag2), LogLevel.Info);
            ModLaunch.McLaunchLog("当前剩余内存：" + Conversions.ToString(Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0) + "G", LogLevel.Info);
            checked
            {
                int num = (int)Math.Floor(unchecked(PageInstanceSetup.GetRam(ModMinecraft.McInstanceSelected) * 1024.0));
                list.Add("-Xmx" + Conversions.ToString(num) + "m");
                int num2 = Settings.Get<int>("LaunchAdvanceGC", null);
                if (ModMinecraft.McInstanceSelected != null && Settings.Get<int>("VersionAdvanceGC", ModMinecraft.McInstanceSelected) > 0)
                {
                    num2 = Settings.Get<int>("VersionAdvanceGC", ModMinecraft.McInstanceSelected) - 1;
                }
                if (num2 != 3)
                {
                    bool flag3 = false;
                    if ((num2 == 0 && ModLaunch.McLaunchJavaSelected.Version.Major < 15) || (num2 == 1 && ModLaunch.McLaunchJavaSelected.Version.Major < 21) || num2 == 2 || num2 == 4)
                    {
                        flag3 = true;
                    }
                    if (Environment.OSVersion.Version.Major < 10 || Environment.OSVersion.Version.Build < 17763)
                    {
                        flag3 = true;
                    }
                    ModLaunch.McLaunchLog(string.Format("GC 设置：{0}，选取 {1}，Java 版本：{2}", num2, flag3 ? "G1GC" : "ZGC", ModLaunch.McLaunchJavaSelected.Version.Major), LogLevel.Info);
                    list = list.Where<string>((ModLaunch._Closure$__.$I51-2 == null) ? (ModLaunch._Closure$__.$I51-2 = (string a) => !a.RegexCheck("( )*-XX:[+-]?(Use\\w+GC|ZGenerational|UseCompactObjectHeaders|G1\\w+Percent|G1\\w+Size|(Max|Min)(GCPauseMillis|HeapFreeRatio))", RegexOptions.None)) : ModLaunch._Closure$__.$I51-2).ToList<string>();
                    list.Add("-XX:+UnlockExperimentalVMOptions");
                    if (ModLaunch.McLaunchJavaSelected.Version.Major >= 24)
                    {
                        list.Add("-XX:+UseCompactObjectHeaders");
                    }
                    if (flag3 && num2 == 4)
                    {
                        list.Add("-XX:+UseG1GC");
                        list.Add("-XX:G1NewSizePercent=20");
                        list.Add("-XX:G1ReservePercent=20");
                        list.Add("-XX:G1HeapRegionSize=32M");
                        list.Add("-XX:MaxGCPauseMillis=50");
                        if (!ModBase.ModeDebug)
                        {
                            list.Add("-XX:+PerfDisableSharedMem");
                        }
                        if (ModLaunch.McLaunchJavaSelected.Version.Major <= 7)
                        {
                            list.Add("-XX:MaxPermSize=512m");
                        }
                        if (ModLaunch.McLaunchJavaSelected.Version.Major == 8)
                        {
                            list.Add("-XX:+ParallelRefProcEnabled");
                        }
                        if (ModLaunch.McLaunchJavaSelected.Version.Major >= 12)
                        {
                            list.Add("-XX:MinHeapFreeRatio=25");
                        }
                        if (ModLaunch.McLaunchJavaSelected.Version.Major >= 12)
                        {
                            list.Add("-XX:MaxHeapFreeRatio=40");
                        }
                    }
                    else if (flag3)
                    {
                        list.Add("-XX:+UseG1GC");
                        list.Add("-XX:G1NewSizePercent=20");
                        list.Add("-XX:G1ReservePercent=20");
                        list.Add("-XX:G1HeapRegionSize=32M");
                        list.Add("-XX:MaxGCPauseMillis=50");
                        if (ModLaunch.McLaunchJavaSelected.Version.Major <= 7)
                        {
                            list.Add("-XX:MaxPermSize=512m");
                        }
                    }
                    else
                    {
                        list.Add("-XX:+UseZGC");
                        if (ModLaunch.McLaunchJavaSelected.Version.Major == 21 || ModLaunch.McLaunchJavaSelected.Version.Major == 22)
                        {
                            list.Add("-XX:+ZGenerational");
                        }
                    }
                }
                list.Remove("-XX:MaxDirectMemorySize=256M");
                list.Add("-Dlog4j2.formatMsgNoLookups=true");
                bool flag4 = ModLaunch.McLaunchJavaSelected.Version.Major >= 18;
                string text4 = (flag4 ? ModLaunch.McLaunchJavaSelected.NativeEncodingName : ModLaunch.McLaunchJavaSelected.FileEncodingName);
                try
                {
                    ModLaunch.McLaunchEncoding = Encoding.GetEncoding(text4.Equals("GBK", StringComparison.OrdinalIgnoreCase) ? "GB18030" : text4);
                }
                catch (Exception ex2)
                {
                    Encoding encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
                    ModLaunch.McLaunchLog(string.Format("无法识别 Java 编码 {0}，已回退为 {1}", text4 ?? "Nothing", encoding.WebName), LogLevel.Warn);
                    ModLaunch.McLaunchEncoding = encoding;
                }
                ModLaunch.McLaunchLog(string.Format("使用编码：{0}", ModLaunch.McLaunchEncoding.WebName), LogLevel.Info);
                string text5 = ((ModLaunch.McLaunchEncoding.CodePage == 54936) ? "GBK" : ModLaunch.McLaunchEncoding.WebName);
                CS$<>8__locals1.$VB$Local_StdoutArgName = ((ModLaunch.McLaunchJavaSelected.Version.Major < 19) ? "sun.stdout.encoding" : "stdout.encoding");
                CS$<>8__locals1.$VB$Local_StderrArgName = ((ModLaunch.McLaunchJavaSelected.Version.Major < 19) ? "sun.stderr.encoding" : "stderr.encoding");
                if (!list.Any<string>((string a) => a.StartsWithF(string.Format("-D{0}=", CS$<>8__locals1.$VB$Local_StdoutArgName), false)))
                {
                    list.Add(string.Format("-D{0}={1}", CS$<>8__locals1.$VB$Local_StdoutArgName, text5));
                }
                if (!list.Any<string>((string a) => a.StartsWithF(string.Format("-D{0}=", CS$<>8__locals1.$VB$Local_StderrArgName), false)))
                {
                    list.Add(string.Format("-D{0}={1}", CS$<>8__locals1.$VB$Local_StderrArgName, text5));
                }
                if (flag4 && !list.Any<string>((ModLaunch._Closure$__.$I51-5 == null) ? (ModLaunch._Closure$__.$I51-5 = (string a) => a.StartsWithF("-Dfile.encoding=", false)) : ModLaunch._Closure$__.$I51-5))
                {
                    list.Add("-Dfile.encoding=COMPAT");
                }
                if (flag)
                {
                    list.Add("-jar");
                    list.Add(LaunchUtils.ExtractPatch("Launch\\JavaWrapper.jar", ModBase.PathPure.Value));
                }
                return ModLaunch.DeduplicateJavaArguments(list, true);
            }
        }

        private static List<string> McLaunchArgumentsGame()
        {
            ModLaunch._Closure$__52-0 CS$<>8__locals1 = new ModLaunch._Closure$__52-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Arg = "";
            JToken jtoken = ModMinecraft.McInstanceSelected.JsonObject["minecraftArguments"];
            if (jtoken != null && jtoken.ToString().Any<char>())
            {
                string text = ModMinecraft.McInstanceSelected.JsonObject["minecraftArguments"].ToString();
                ModLaunch.McLaunchLog("旧版游戏参数：" + text, LogLevel.Info);
                text += " --height ${resolution_height} --width ${resolution_width}";
                CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg + " " + text;
            }
            if (ModMinecraft.McInstanceSelected.JsonObject["arguments"] != null && ModMinecraft.McInstanceSelected.JsonObject["arguments"]["game"] != null)
            {
                ModMinecraft.McInstance mcInstance = ModMinecraft.McInstanceSelected;
                VB$AnonymousDelegate_18<string> vb$AnonymousDelegate_ = delegate(string a)
                {
                    CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg + " " + ((a.Contains(" ") && !a.StartsWithF("\"", false) && !a.EndsWithF("\"", false)) ? string.Format("\"{0}\"", a.Replace("\"", "\\\"")) : a);
                };
                for (;;)
                {
                    ModLaunch.McLaunchLog(string.Format("新版游戏参数：从版本 JSON 获取（{0}）", mcInstance.Name), LogLevel.Info);
                    if (mcInstance.JsonObject["arguments"] != null && mcInstance.JsonObject["arguments"]["game"] != null)
                    {
                        try
                        {
                            foreach (JToken jtoken2 in mcInstance.JsonObject["arguments"]["game"])
                            {
                                JTokenType type = jtoken2.Type;
                                if (type != 1)
                                {
                                    if (type == 8)
                                    {
                                        vb$AnonymousDelegate_(jtoken2.ToString());
                                    }
                                }
                                else
                                {
                                    JObject jobject = (JObject)jtoken2;
                                    if ((!jobject.ContainsKey("rules") || ModMinecraft.McJsonRuleCheck(jobject["rules"])) && jobject.ContainsKey("value"))
                                    {
                                        if (jobject["value"].Type == 8)
                                        {
                                            vb$AnonymousDelegate_(jobject["value"].ToString());
                                        }
                                        else
                                        {
                                            try
                                            {
                                                foreach (JToken jtoken3 in jobject["value"])
                                                {
                                                    vb$AnonymousDelegate_(jtoken3.ToString());
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
                    if (Operators.CompareString(mcInstance.InheritName, "", false) == 0)
                    {
                        break;
                    }
                    mcInstance = new ModMinecraft.McInstance(mcInstance.InheritName);
                }
            }
            string text2 = Settings.Get<string>("VersionAdvanceGame", ModMinecraft.McInstanceSelected);
            if (Operators.CompareString(text2, "", false) == 0)
            {
                text2 = Settings.Get<string>("LaunchAdvanceGame", null);
            }
            if (Operators.CompareString(text2, "", false) != 0)
            {
                text2 = ModMain.ArgumentReplace(text2, null, true);
            }
            CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg + " " + text2;
            if ((ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.Forge) || ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader)) && ModMinecraft.McInstanceSelected.Version.HasOptiFine && !ModMinecraft.McInstanceSelected.Version.OptiFineAsMod)
            {
                if (CS$<>8__locals1.$VB$Local_Arg.Contains("--tweakClass optifine.OptiFineForgeTweaker"))
                {
                    ModLaunch.McLaunchLog("发现正确的 OptiFineForge TweakClass，目前的游戏参数：" + CS$<>8__locals1.$VB$Local_Arg, LogLevel.Info);
                    CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg.Replace(" --tweakClass optifine.OptiFineForgeTweaker", "") + " --tweakClass optifine.OptiFineForgeTweaker";
                }
                if (CS$<>8__locals1.$VB$Local_Arg.Contains("--tweakClass optifine.OptiFineTweaker"))
                {
                    ModLaunch.McLaunchLog("发现错误的 OptiFineForge TweakClass，目前的游戏参数：" + CS$<>8__locals1.$VB$Local_Arg, LogLevel.Info);
                    CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg.Replace(" --tweakClass optifine.OptiFineTweaker", "") + " --tweakClass optifine.OptiFineForgeTweaker";
                    try
                    {
                        FileUtils.Write(ModMinecraft.McInstanceSelected.GetJsonPath(), FileUtils.ReadAsString(ModMinecraft.McInstanceSelected.GetJsonPath(), null, null).Replace("optifine.OptiFineTweaker", "optifine.OptiFineForgeTweaker"), null);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "替换 OptiFineForge TweakClass 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    }
                }
            }
            List<string> list = ModLaunch.DeduplicateJavaArguments(ModLaunch.SplitJavaArguments(CS$<>8__locals1.$VB$Local_Arg).ToList<string>(), false);
            list.AddRange(ModLaunch.CurrentLaunchOptions.ExtraGameArgs);
            if (Settings.Get<int>("LaunchArgumentWindowType", null) == 0)
            {
                list.Add("--fullscreen");
            }
            string text3 = (string.IsNullOrEmpty(ModLaunch.CurrentLaunchOptions.ServerIp) ? Settings.Get<string>("VersionServerEnter", ModMinecraft.McInstanceSelected) : ModLaunch.CurrentLaunchOptions.ServerIp);
            if (text3.Any<char>())
            {
                if (DateTime.Compare(ModMinecraft.McInstanceSelected.ReleaseTime, new DateTime(2023, 4, 4)) > 0)
                {
                    list.Add("--quickPlayMultiplayer");
                    list.Add(text3);
                }
                else
                {
                    if (text3.Contains(":"))
                    {
                        list.Add("--server");
                        list.Add(text3.Split(":", false)[0]);
                        list.Add("--port");
                        list.Add(text3.Split(":", false)[1]);
                    }
                    else
                    {
                        list.Add("--server");
                        list.Add(text3);
                        list.Add("--port");
                        list.Add("25565");
                    }
                    if (ModMinecraft.McInstanceSelected.Version.HasOptiFine)
                    {
                        ModMain.Hint("OptiFine 与自动进入服务器可能不兼容，有概率导致材质丢失甚至游戏崩溃！", ModMain.HintType.Red, true);
                    }
                }
            }
            return ModLaunch.DeduplicateJavaArguments(list, false);
        }

        [return: TupleElementNames(new string[] { "Key", "Value" })]
        private static IEnumerable<ValueTuple<string, string>> McLaunchArgumentsReplace(List<string> Arguments)
        {
            ModLaunch.VB$StateMachine_53_McLaunchArgumentsReplace vb$StateMachine_53_McLaunchArgumentsReplace = new ModLaunch.VB$StateMachine_53_McLaunchArgumentsReplace(-2);
            vb$StateMachine_53_McLaunchArgumentsReplace.$P_Arguments = Arguments;
            return vb$StateMachine_53_McLaunchArgumentsReplace;
        }

        private static IEnumerable<string> SplitJavaArguments(string Arg)
        {
            ModLaunch.VB$StateMachine_54_SplitJavaArguments vb$StateMachine_54_SplitJavaArguments = new ModLaunch.VB$StateMachine_54_SplitJavaArguments(-2);
            vb$StateMachine_54_SplitJavaArguments.$P_Arg = Arg;
            return vb$StateMachine_54_SplitJavaArguments;
        }

        private static List<string> DeduplicateJavaArguments(List<string> Args, bool IsJVMArgs)
        {
            List<string> list = new List<string>();
            int i = 0;
            IL_018C:
            checked
            {
                while (i < Args.Count)
                {
                    string text = Args[i];
                    if (!text.StartsWithF("-", false) || i + 1 >= Args.Count || (Args[i + 1].StartsWithF("-", false) && ModBase.Val(Args[i + 1].AfterFirst("-", false)) == 0.0))
                    {
                        i++;
                        if (list.Contains(text))
                        {
                            ModLaunch.McLaunchLog(string.Format("已去重单个{0}参数：{1}", IsJVMArgs ? " JVM " : "游戏", text), LogLevel.Info);
                        }
                        else
                        {
                            list.Add(text);
                        }
                    }
                    else
                    {
                        string text2 = Args[i + 1];
                        i += 2;
                        int num = list.Count - 2;
                        for (int j = 0; j <= num; j++)
                        {
                            if (Operators.CompareString(list[j], text, false) == 0)
                            {
                                if (!IsJVMArgs && Operators.CompareString(text, "--tweakClass", false) != 0)
                                {
                                    ModLaunch.McLaunchLog(string.Format("已覆盖重复的{0}参数：{1} {2} → {3} {4}", new object[]
                                    {
                                        IsJVMArgs ? " JVM " : "游戏",
                                        text,
                                        list[j + 1],
                                        text,
                                        text2
                                    }), LogLevel.Info);
                                    list[j + 1] = text2;
                                    goto IL_018C;
                                }
                                if (Operators.CompareString(list[j + 1], text2, false) == 0)
                                {
                                    ModLaunch.McLaunchLog(string.Format("已去重{0}参数对：{1} {2}", IsJVMArgs ? " JVM " : "游戏", text, text2), LogLevel.Info);
                                    goto IL_018C;
                                }
                            }
                        }
                        list.Add(text);
                        list.Add(text2);
                    }
                }
                return list;
            }
        }

        private static void McLaunchNatives(ModLoader.LoaderTask<int, int> Loader)
        {
            string nativesFolder = ModLaunch.GetNativesFolder(null);
            DirectoryUtils.Create(nativesFolder);
            ModLaunch.McLaunchLog("正在解压 Natives 文件", LogLevel.Info);
            List<string> list = new List<string>();
            try
            {
                foreach (ModMinecraft.McLibEntry mcLibEntry in ModMinecraft.McLibListGet(ModMinecraft.McInstanceSelected, true))
                {
                    if (mcLibEntry.IsNatives)
                    {
                        ZipArchive zipArchive;
                        try
                        {
                            zipArchive = FileUtils.OpenZip(mcLibEntry.LocalPath, default(CancellationToken));
                        }
                        catch (InvalidDataException ex)
                        {
                            Logger.Warn(ex, string.Format("打开 Natives 文件失败（{0}）", mcLibEntry.LocalPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                            FileUtils.Delete(mcLibEntry.LocalPath, false);
                            throw new Exception("无法打开 Natives 文件（" + mcLibEntry.LocalPath + "），该文件可能已损坏，请重新尝试启动游戏");
                        }
                        try
                        {
                            foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                            {
                                string fullName = zipArchiveEntry.FullName;
                                if (Operators.CompareString(PathUtils.GetExtension(fullName), "dll", false) == 0)
                                {
                                    string text = nativesFolder + fullName;
                                    list.Add(text);
                                    FileInfo info = FileUtils.GetInfo(text);
                                    if (info.Exists)
                                    {
                                        if (info.Length == zipArchiveEntry.Length)
                                        {
                                            if (ModBase.ModeDebug)
                                            {
                                                ModLaunch.McLaunchLog("无需解压：" + text, LogLevel.Info);
                                                continue;
                                            }
                                            continue;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                FileUtils.Delete(text, false);
                                            }
                                            catch (UnauthorizedAccessException ex2)
                                            {
                                                ModLaunch.McLaunchLog("删除原 dll 访问被拒绝，这通常代表有一个 MC 正在运行，跳过解压：" + text, LogLevel.Info);
                                                ModLaunch.McLaunchLog("实际的错误信息：" + ex2.GetDisplay(true), LogLevel.Info);
                                                break;
                                            }
                                        }
                                    }
                                    FileUtils.Write(text, zipArchiveEntry.Open());
                                    ModLaunch.McLaunchLog("已解压：" + text, LogLevel.Info);
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<ZipArchiveEntry> enumerator2;
                            if (enumerator2 != null)
                            {
                                enumerator2.Dispose();
                            }
                        }
                        if (zipArchive != null)
                        {
                            zipArchive.Dispose();
                        }
                    }
                }
            }
            finally
            {
                List<ModMinecraft.McLibEntry>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            try
            {
                foreach (string text2 in DirectoryUtils.EnumerateFiles(nativesFolder, false, "*"))
                {
                    if (!list.Contains(text2))
                    {
                        try
                        {
                            ModLaunch.McLaunchLog("删除：" + text2, LogLevel.Info);
                            FileUtils.Delete(text2, false);
                        }
                        catch (UnauthorizedAccessException ex3)
                        {
                            ModLaunch.McLaunchLog("删除多余文件访问被拒绝，跳过删除步骤", LogLevel.Info);
                            ModLaunch.McLaunchLog("实际的错误信息：" + ex3.GetDisplay(true), LogLevel.Info);
                            break;
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<string> enumerator3;
                if (enumerator3 != null)
                {
                    enumerator3.Dispose();
                }
            }
        }

        public static string GetNativesFolder(ModMinecraft.McInstance Instance = null)
        {
            if (Instance == null)
            {
                Instance = ModMinecraft.McInstanceSelected;
            }
            string text = PathUtils.AddSlashSuffix(PathUtils.ToShortPath(Instance.PathVersion)) + Instance.Name + "-natives\\";
            if (!ModBase.IsGBKEncoding && !text.IsAsciiOnly())
            {
                text = Paths.AppData + ".minecraft\\bin\\natives\\";
                if (!text.IsAsciiOnly())
                {
                    text = ModBase.OsDrive + "ProgramData\\PCL\\natives\\";
                }
            }
            DirectoryUtils.Create(text);
            return PathUtils.AddSlashSuffix(PathUtils.ToShortPath(text));
        }

        private static void McLaunchPrerun()
        {
            if (Settings.Get<bool>("LaunchAdvanceGraphicCard", null))
            {
                try
                {
                    ModMain.SetGPUPreference(ModLaunch.McLaunchJavaSelected.JavaExePath);
                    ModMain.SetGPUPreference(ModBase.PathExe);
                }
                catch (Exception ex)
                {
                    if (WindowsUtils.HasAdminRole())
                    {
                        Logger.Warn(ex, "直接调整显卡设置失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    }
                    else
                    {
                        Logger.Warn(ex, "直接调整显卡设置失败，将以管理员权限重启 PCL 再次尝试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                        try
                        {
                            if (ModBase.RunAsAdmin(string.Format("--gpu \"{0}\"", ModLaunch.McLaunchJavaSelected.JavaExePath)) != 5)
                            {
                                throw new Exception("调整过程中出现异常");
                            }
                            ModLaunch.McLaunchLog("以管理员权限重启 PCL 并调整显卡设置成功", LogLevel.Info);
                        }
                        catch (Exception ex2)
                        {
                            Logger.Error(ex2, "调整显卡设置失败，Minecraft 可能会使用默认显卡运行", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                        }
                    }
                }
            }
            try
            {
                if (Operators.CompareString(ModLaunch.McLoginLoader.Output.Type, "Microsoft", false) == 0)
                {
                    ModMinecraft.McFolderLauncherProfilesJsonCreate(ModMinecraft.McFolderSelected);
                    JObject jobject = (JObject)string.Concat(new string[]
                    {
                        "\r\n            {\r\n              \"authenticationDatabase\": {\r\n                \"00000111112222233333444445555566\": {\r\n                  \"username\": \"",
                        ModLaunch.McLoginLoader.Output.Name.Replace("\"", "-"),
                        "\",\r\n                  \"profiles\": {\r\n                    \"66666555554444433333222221111100\": {\r\n                        \"displayName\": \"",
                        ModLaunch.McLoginLoader.Output.Name,
                        "\"\r\n                    }\r\n                  }\r\n                }\r\n              },\r\n              \"clientToken\": \"",
                        ModLaunch.McLoginLoader.Output.ClientToken,
                        "\",\r\n              \"selectedUser\": {\r\n                \"account\": \"00000111112222233333444445555566\", \r\n                \"profile\": \"66666555554444433333222221111100\"\r\n              }\r\n            }"
                    }).DeserializeJson();
                    JObject jobject2 = (JObject)FileUtils.ReadAsJson(ModMinecraft.McFolderSelected + "launcher_profiles.json", null, null);
                    jobject2.Merge(jobject);
                    FileUtils.Write(ModMinecraft.McFolderSelected + "launcher_profiles.json", jobject2.ToString(), Encoding.GetEncoding("GB18030"));
                    ModLaunch.McLaunchLog("已更新 launcher_profiles.json", LogLevel.Info);
                }
            }
            catch (Exception ex3)
            {
                Logger.Warn(ex3, "更新 launcher_profiles.json 失败，将在删除文件后重试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                try
                {
                    FileUtils.Delete(ModMinecraft.McFolderSelected + "launcher_profiles.json", false);
                    ModMinecraft.McFolderLauncherProfilesJsonCreate(ModMinecraft.McFolderSelected);
                    JObject jobject3 = (JObject)string.Concat(new string[]
                    {
                        "\r\n                    {\r\n                      \"authenticationDatabase\": {\r\n                        \"00000111112222233333444445555566\": {\r\n                          \"username\": \"",
                        ModLaunch.McLoginLoader.Output.Name.Replace("\"", "-"),
                        "\",\r\n                          \"profiles\": {\r\n                            \"66666555554444433333222221111100\": {\r\n                                \"displayName\": \"",
                        ModLaunch.McLoginLoader.Output.Name,
                        "\"\r\n                            }\r\n                          }\r\n                        }\r\n                      },\r\n                      \"clientToken\": \"",
                        ModLaunch.McLoginLoader.Output.ClientToken,
                        "\",\r\n                      \"selectedUser\": {\r\n                        \"account\": \"00000111112222233333444445555566\", \r\n                        \"profile\": \"66666555554444433333222221111100\"\r\n                      }\r\n                    }"
                    }).DeserializeJson();
                    JObject jobject4 = (JObject)FileUtils.ReadAsJson(ModMinecraft.McFolderSelected + "launcher_profiles.json", null, null);
                    jobject4.Merge(jobject3);
                    FileUtils.Write(ModMinecraft.McFolderSelected + "launcher_profiles.json", jobject4.ToString(), Encoding.GetEncoding("GB18030"));
                    ModLaunch.McLaunchLog("已在删除后更新 launcher_profiles.json", LogLevel.Info);
                }
                catch (Exception ex4)
                {
                    Logger.Error(ex4, "更新 launcher_profiles.json 失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                }
            }
            string text = ModMinecraft.McInstanceSelected.PathIndie + "options.txt";
            if (!FileUtils.Exists(text))
            {
                string text2 = ModMinecraft.McInstanceSelected.PathIndie + "config\\yosbr\\options.txt";
                if (FileUtils.Exists(text2))
                {
                    ModLaunch.McLaunchLog("将修改 Yosbr Mod 中的 options.txt", LogLevel.Info);
                    text = text2;
                    ModBase.WriteIni(text, "lang", "none");
                }
            }
            checked
            {
                try
                {
                    string text3 = ModBase.ReadIni(text, "lang", "none");
                    string text4 = ((Operators.CompareString(text3, "none", false) == 0 || !DirectoryUtils.Exists(ModMinecraft.McInstanceSelected.PathIndie + "saves")) ? (Settings.Get<bool>("ToolHelpChinese", null) ? "zh_cn" : "en_us") : text3.Lower());
                    if (!ModMinecraft.McInstanceSelected.Version.Valid || ModMinecraft.McInstanceSelected.Version.Vanilla.Major <= 10)
                    {
                        text4 = text4.Substring(0, text4.Length - 2) + text4.Substring(text4.Length - 2).Upper();
                    }
                    if (Operators.CompareString(text3, text4, false) == 0)
                    {
                        ModLaunch.McLaunchLog(string.Format("需要的语言为 {0}，当前语言为 {1}，无需修改", text4, text3), LogLevel.Info);
                    }
                    else
                    {
                        ModBase.WriteIni(text, "lang", "-");
                        ModBase.WriteIni(text, "lang", text4);
                        ModLaunch.McLaunchLog(string.Format("已将语言从 {0} 修改为 {1}", text3, text4), LogLevel.Info);
                    }
                    int num = Settings.Get<int>("LaunchArgumentWindowType", null);
                    if (num != 0)
                    {
                        if (num != 1)
                        {
                            ModBase.WriteIni(text, "fullscreen", "false");
                        }
                    }
                    else
                    {
                        ModBase.WriteIni(text, "fullscreen", "true");
                    }
                }
                catch (Exception ex5)
                {
                    Logger.Error(ex5, "更新 options.txt 失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                }
                if (ModMinecraft.McInstanceSelected.Version.Vanilla.Major <= 7 && ModMinecraft.McInstanceSelected.Version.Vanilla.Major >= 2 && ModLaunch.McLoginLoader.Input.Type == ModLaunch.McLoginType.Legacy && (Settings.Get<int>("LaunchSkinType", null) == 2 || (Settings.Get<int>("LaunchSkinType", null) == 4 && Settings.Get<bool>("LaunchSkinSlim", null))))
                {
                    ModMain.Hint("此 Minecraft 版本尚不支持 Alex 皮肤，你的皮肤可能会显示为 Steve！", ModMain.HintType.Red, true);
                }
                try
                {
                    string text5 = ModMinecraft.McInstanceSelected.PathIndie + "resourcepacks\\PCL2 Skin.zip";
                    bool flag = ModMinecraft.McInstanceSelected.Version.Vanilla.Major >= 13 || ModMinecraft.McInstanceSelected.Version.Vanilla.Major < 6;
                    if (ModLaunch.McLoginLoader.Input.Type == ModLaunch.McLoginType.Legacy && Settings.Get<int>("LaunchSkinType", null) == 4 && FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png"))
                    {
                        string text6 = ModBase.PathTemp + "pack.mcmeta";
                        string text7 = ModBase.PathTemp + "pack.png";
                        if (ModMinecraft.McInstanceSelected.Version.Vanilla.Major < 6)
                        {
                            ModLaunch.McLaunchLog("Minecraft 版本过老，尚不支持自定义离线皮肤", LogLevel.Info);
                        }
                        else
                        {
                            JObject jsonVersion = ModMinecraft.McInstanceSelected.JsonVersion;
                            JToken jtoken = ((jsonVersion != null) ? jsonVersion["pack_version"] : null);
                            if (jtoken is JObject)
                            {
                                jtoken = ((JObject)jtoken)["resource"];
                            }
                            int num2 = (int)Math.Round(ModBase.Val((jtoken != null) ? jtoken.ToString() : null));
                            if (num2 <= 0)
                            {
                                switch (ModMinecraft.McInstanceSelected.Version.Vanilla.Major)
                                {
                                case 6:
                                case 7:
                                case 8:
                                    num2 = 1;
                                    break;
                                case 9:
                                case 10:
                                    num2 = 2;
                                    break;
                                case 11:
                                case 12:
                                    num2 = 3;
                                    break;
                                case 13:
                                case 14:
                                    num2 = 4;
                                    break;
                                case 15:
                                    num2 = 5;
                                    break;
                                case 16:
                                    num2 = ((ModMinecraft.McInstanceSelected.Version.Vanilla.Build <= 1) ? 5 : 6);
                                    break;
                                case 17:
                                    num2 = 7;
                                    break;
                                case 18:
                                    num2 = 8;
                                    break;
                                default:
                                    num2 = 17;
                                    break;
                                }
                            }
                            ModLaunch.McLaunchLog(string.Format("正在构建自定义皮肤资源包，版本号 {0}，MC 版本 {1}", num2, ModMinecraft.McInstanceSelected.Version.Vanilla), LogLevel.Info);
                            new MyBitmap(ModBase.PathImage + "Heads/Logo.png").Save(text7);
                            FileUtils.Write(text6, "{\"pack\":{\"pack_format\":" + Conversions.ToString(num2) + ",\"description\":\"PCL 自定义离线皮肤资源包\"}}", null);
                            MyBitmap myBitmap = new MyBitmap(Paths.AppDataThenName + "CustomSkin.png");
                            if ((ModMinecraft.McInstanceSelected.Version.Vanilla.Major == 6 || ModMinecraft.McInstanceSelected.Version.Vanilla.Major == 7) && myBitmap.Pic.Height == 64)
                            {
                                ModLaunch.McLaunchLog("该 Minecraft 版本不支持双层皮肤，已进行裁剪", LogLevel.Info);
                                myBitmap = myBitmap.Clip(0, 0, 64, 32);
                            }
                            myBitmap.Save(Paths.Base + "PCL\\CustomSkin_Cliped.png");
                            Dictionary<string, string> dictionary = new Dictionary<string, string>
                            {
                                { "pack.mcmeta", text6 },
                                { "pack.png", text7 }
                            };
                            if (ModMinecraft.McInstanceSelected.Version.Vanilla >= new Version(19, 0, 3))
                            {
                                foreach (string text8 in new string[] { "alex", "ari", "efe", "kai", "makena", "noor", "steve", "sunny", "zuri" })
                                {
                                    dictionary.Add(string.Format("assets/minecraft/textures/entity/player/{0}/{1}.png", Settings.Get<bool>("LaunchSkinSlim", null) ? "slim" : "wide", text8), Paths.Base + "PCL\\CustomSkin_Cliped.png");
                                }
                            }
                            else
                            {
                                dictionary.Add(string.Format("assets/minecraft/textures/entity/{0}", Settings.Get<bool>("LaunchSkinSlim", null) ? "alex.png" : "steve.png"), Paths.Base + "PCL\\CustomSkin_Cliped.png");
                            }
                            FileUtils.CreateZipFromFiles(text5, dictionary);
                            FileUtils.Delete(Paths.Base + "PCL\\CustomSkin_Cliped.png", false);
                            ModBase.IniClearCache(text);
                            string text9 = ModBase.ReadIni(text, "resourcePacks", "[]").TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
                            if (flag)
                            {
                                if (Operators.CompareString(text9, "", false) == 0)
                                {
                                    text9 = "\"vanilla\"";
                                }
                                List<string> list = new List<string>(text9.Split(",", false));
                                List<string> list2 = new List<string>();
                                try
                                {
                                    foreach (string text10 in list)
                                    {
                                        if (Operators.CompareString(text10, "\"file/PCL2 Skin.zip\"", false) != 0 && Operators.CompareString(text10, "", false) != 0)
                                        {
                                            list2.Add(text10);
                                        }
                                    }
                                }
                                finally
                                {
                                    List<string>.Enumerator enumerator;
                                    ((IDisposable)enumerator).Dispose();
                                }
                                list2.Add("\"file/PCL2 Skin.zip\"");
                                string text11 = "[" + list2.Join(',') + "]";
                                ModBase.WriteIni(text, "resourcePacks", text11);
                            }
                            else
                            {
                                List<string> list3 = new List<string>(text9.Split(",", false));
                                List<string> list4 = new List<string>();
                                try
                                {
                                    foreach (string text12 in list3)
                                    {
                                        if (Operators.CompareString(text12, "\"PCL2 Skin.zip\"", false) != 0 && Operators.CompareString(text12, "", false) != 0)
                                        {
                                            list4.Add(text12);
                                        }
                                    }
                                }
                                finally
                                {
                                    List<string>.Enumerator enumerator2;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                                list4.Add("\"PCL2 Skin.zip\"");
                                string text13 = "[" + list4.Join(',') + "]";
                                ModBase.WriteIni(text, "resourcePacks", text13);
                            }
                        }
                    }
                    else if (FileUtils.Exists(text5))
                    {
                        ModLaunch.McLaunchLog("正在清空自定义皮肤资源包", LogLevel.Info);
                        FileUtils.Delete(text5, false);
                        ModBase.IniClearCache(text);
                        string text14 = ModBase.ReadIni(text, "resourcePacks", "[]").TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
                        if (flag)
                        {
                            if (Operators.CompareString(text14, "", false) == 0)
                            {
                                text14 = "\"vanilla\"";
                            }
                            List<string> list5 = new List<string>(text14.Split(",", false));
                            list5.Remove("\"file/PCL2 Skin.zip\"");
                            string text15 = "[" + list5.Join(',') + "]";
                            ModBase.WriteIni(text, "resourcePacks", text15);
                        }
                        else
                        {
                            List<string> list6 = new List<string>(text14.Split(",", false));
                            list6.Remove("\"PCL2 Skin.zip\"");
                            string text16 = "[" + list6.Join(',') + "]";
                            ModBase.WriteIni(text, "resourcePacks", text16);
                        }
                    }
                }
                catch (Exception ex6)
                {
                    Logger.Error(ex6, "离线皮肤资源包设置失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                }
            }
        }

        private static void McLaunchCustom(ModLoader.LoaderTask<int, int> Loader)
        {
            string text = Settings.Get<string>("LaunchAdvanceRun", null);
            if (Operators.CompareString(text, "", false) != 0)
            {
                text = ModMain.ArgumentReplace(text, null, true);
            }
            string text2 = Settings.Get<string>("VersionAdvanceRun", ModMinecraft.McInstanceSelected);
            if (Operators.CompareString(text2, "", false) != 0)
            {
                text2 = ModMain.ArgumentReplace(text2, null, true);
            }
            try
            {
                string text3 = string.Concat(new string[]
                {
                    string.Format("chcp {0}>nul", (ModLaunch.McLaunchEncoding.CodePage == 54936 && ModBase.IsGBKEncoding) ? 936 : ModLaunch.McLaunchEncoding.CodePage),
                    "\r\n@echo off\r\n",
                    string.Format("title 启动 - {0}", ModMinecraft.McInstanceSelected.Name),
                    "\r\necho 游戏正在启动，请稍候。\r\n",
                    string.Format("cd /D \"{0}\"", PathUtils.ToShortPath(ModMinecraft.McInstanceSelected.PathIndie)),
                    "\r\n",
                    text,
                    "\r\n",
                    text2,
                    "\r\n",
                    string.Format("\"{0}\" {1}", ModLaunch.McLaunchJavaSelected.JavaExePath, ModLaunch.McLaunchArgument),
                    "\r\necho 游戏已退出。\r\npause"
                });
                FileUtils.Write(ModLaunch.CurrentLaunchOptions.SaveBatch ?? (Paths.Base + "PCL\\LatestLaunch.bat"), ModMinecraft.FilterAccessToken(text3, 'F').Replace("%", "%%"), ModLaunch.McLaunchEncoding);
                if (ModLaunch.CurrentLaunchOptions.SaveBatch != null)
                {
                    ModLaunch.McLaunchLog("导出启动脚本完成，强制结束启动过程", LogLevel.Info);
                    ModLaunch.CanceledHint = "导出启动脚本成功！";
                    ModBase.OpenExplorer(ModLaunch.CurrentLaunchOptions.SaveBatch);
                    Loader.Parent.Cancel();
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "输出启动脚本失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                if (ModLaunch.CurrentLaunchOptions.SaveBatch != null)
                {
                    throw ex;
                }
            }
            if (Operators.CompareString(text, "", false) != 0)
            {
                ModLaunch.McLaunchLog("正在执行全局自定义命令：" + text, LogLevel.Info);
                Process process = null;
                try
                {
                    process = ModBase.StartProcess(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = string.Format("/c \"{0}\"", text),
                        WorkingDirectory = ModMinecraft.McFolderSelected,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    if (Settings.Get<bool>("LaunchAdvanceRunWait", null))
                    {
                        while (!process.HasExited && !Loader.IsCanceled)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, "执行全局自定义命令失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                }
                finally
                {
                    if (process != null && !process.HasExited && Loader.IsCanceled)
                    {
                        ModLaunch.McLaunchLog("由于取消启动，已强制结束自定义命令 CMD 进程", LogLevel.Info);
                        process.Kill();
                    }
                }
            }
            if (Operators.CompareString(text2, "", false) != 0)
            {
                ModLaunch.McLaunchLog("正在执行版本自定义命令：" + text2, LogLevel.Info);
                Process process2 = null;
                try
                {
                    process2 = ModBase.StartProcess(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = string.Format("/c \"{0}\"", text2),
                        WorkingDirectory = ModMinecraft.McFolderSelected,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    if (Settings.Get<bool>("VersionAdvanceRunWait", ModMinecraft.McInstanceSelected))
                    {
                        while (!process2.HasExited && !Loader.IsCanceled)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (Exception ex3)
                {
                    Logger.Error(ex3, "执行版本自定义命令失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                }
                finally
                {
                    if (process2 != null && !process2.HasExited && Loader.IsCanceled)
                    {
                        ModLaunch.McLaunchLog("由于取消启动，已强制结束自定义命令 CMD 进程", LogLevel.Info);
                        process2.Kill();
                    }
                }
            }
        }

        private static void McLaunchRun(ModLoader.LoaderTask<int, Process> Loader)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(ModLaunch.McLaunchJavaSelected.JavaExePath);
            string text = processStartInfo.EnvironmentVariables["Path"];
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(text))
            {
                list.AddRange(text.Split(";", false));
            }
            list.Add(PathUtils.ToShortPath(ModLaunch.McLaunchJavaSelected.Folder));
            processStartInfo.EnvironmentVariables["Path"] = list.Distinct<string>().Join(';');
            processStartInfo.EnvironmentVariables["appdata"] = PathUtils.ToShortPath(ModMinecraft.McFolderSelected);
            processStartInfo.WorkingDirectory = ModMinecraft.McInstanceSelected.PathIndie;
            processStartInfo.UseShellExecute = false;
            processStartInfo.StandardOutputEncoding = ModLaunch.McLaunchEncoding;
            processStartInfo.StandardErrorEncoding = ModLaunch.McLaunchEncoding;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = ModLaunch.McLaunchArgument;
            Process process = ModBase.StartProcess(processStartInfo);
            ModLaunch.McLaunchLog("已启动游戏进程：" + ModLaunch.McLaunchJavaSelected.JavaExePath, LogLevel.Info);
            if (Loader.IsCanceled)
            {
                ModLaunch.McLaunchLog("由于取消启动，已强制结束游戏进程", LogLevel.Info);
                process.Kill();
                return;
            }
            Loader.Output = process;
            ModLaunch.McLaunchProcess = process;
            try
            {
                process.PriorityBoostEnabled = true;
                int num = Settings.Get<int>("LaunchArgumentPriority", null);
                if (num != 0)
                {
                    if (num == 2)
                    {
                        process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    }
                }
                else
                {
                    process.PriorityClass = ProcessPriorityClass.AboveNormal;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "设置进程优先级失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
            }
        }

        private static void McLaunchWait(ModLoader.LoaderTask<Process, int> Loader)
        {
            ModLaunch.McLaunchLog("", LogLevel.Info);
            ModLaunch.McLaunchLog("~ 基础参数 ~", LogLevel.Info);
            ModLaunch.McLaunchLog("PCL 版本：快照版 2.99.0.0 (" + Conversions.ToString(414) + ")", LogLevel.Info);
            ModLaunch.McLaunchLog(string.Format("游戏版本：{0}（{1}，Drop {2}{3}）", new object[]
            {
                ModMinecraft.McInstanceSelected.VersionDisplayName(),
                ModMinecraft.McInstanceSelected.Version.Vanilla,
                ModMinecraft.McInstanceSelected.Version.Drop,
                ModMinecraft.McInstanceSelected.Version.Reliable ? "" : "，无法完全确定"
            }), LogLevel.Info);
            ModLaunch.McLaunchLog("资源版本：" + ModMinecraft.McAssetsGetIndexName(ModMinecraft.McInstanceSelected), LogLevel.Info);
            ModLaunch.McLaunchLog("版本继承：" + ((Operators.CompareString(ModMinecraft.McInstanceSelected.InheritName, "", false) == 0) ? "无" : ModMinecraft.McInstanceSelected.InheritName), LogLevel.Info);
            ModLaunch.McLaunchLog(string.Concat(new string[]
            {
                "分配的内存：",
                Conversions.ToString(PageInstanceSetup.GetRam(ModMinecraft.McInstanceSelected)),
                " GB（",
                Conversions.ToString(Math.Round(PageInstanceSetup.GetRam(ModMinecraft.McInstanceSelected) * 1024.0)),
                " MB）"
            }), LogLevel.Info);
            ModLaunch.McLaunchLog("MC 文件夹：" + ModMinecraft.McFolderSelected, LogLevel.Info);
            ModLaunch.McLaunchLog("版本文件夹：" + ModMinecraft.McInstanceSelected.PathVersion, LogLevel.Info);
            ModLaunch.McLaunchLog("版本隔离：" + Conversions.ToString(Operators.CompareString(ModMinecraft.McInstanceSelected.PathIndie, ModMinecraft.McInstanceSelected.PathVersion, false) == 0), LogLevel.Info);
            ModLaunch.McLaunchLog("HMCL 格式：" + Conversions.ToString(ModMinecraft.McInstanceSelected.IsHmclFormatJson), LogLevel.Info);
            ModLaunch.McLaunchLog("Java 信息：" + ((ModLaunch.McLaunchJavaSelected != null) ? ModLaunch.McLaunchJavaSelected.ToString() : "无可用 Java"), LogLevel.Info);
            ModLaunch.McLaunchLog("Natives 文件夹：" + ModLaunch.GetNativesFolder(null), LogLevel.Info);
            ModLaunch.McLaunchLog("", LogLevel.Info);
            ModLaunch.McLaunchLog("~ 登录参数 ~", LogLevel.Info);
            ModLaunch.McLaunchLog("玩家用户名：" + ModLaunch.McLoginLoader.Output.Name, LogLevel.Info);
            ModLaunch.McLaunchLog("AccessToken：" + ModLaunch.McLoginLoader.Output.AccessToken, LogLevel.Info);
            ModLaunch.McLaunchLog("ClientToken：" + ModLaunch.McLoginLoader.Output.ClientToken, LogLevel.Info);
            ModLaunch.McLaunchLog("UUID：" + ModLaunch.McLoginLoader.Output.Uuid, LogLevel.Info);
            ModLaunch.McLaunchLog("登录方式：" + ModLaunch.McLoginLoader.Output.Type, LogLevel.Info);
            ModLaunch.McLaunchLog("", LogLevel.Info);
            string text = Settings.Get<string>("VersionArgumentTitle", ModMinecraft.McInstanceSelected);
            if (Operators.CompareString(text, "", false) == 0)
            {
                text = Settings.Get<string>("LaunchArgumentTitle", null);
            }
            text = ModMain.ArgumentReplace(text, null, false);
            ModWatcher.Watcher watcher = new ModWatcher.Watcher(Loader, ModMinecraft.McInstanceSelected, text);
            ModLaunch.McLaunchWatcher = watcher;
            while (watcher.State == ModWatcher.Watcher.MinecraftState.Loading)
            {
                Thread.Sleep(100);
            }
            if (watcher.State == ModWatcher.Watcher.MinecraftState.Crashed)
            {
                throw new OperationCanceledException();
            }
        }

        private static void McLaunchEnd()
        {
            ModLaunch.McLaunchLog("开始启动结束处理", LogLevel.Info);
            if (Settings.Get<bool>("UiMusicStop", null))
            {
                ModBase.RunInUi((ModLaunch._Closure$__.$I62-0 == null) ? (ModLaunch._Closure$__.$I62-0 = delegate
                {
                    if (ModMusic.MusicPause())
                    {
                        Logger.Info("已根据设置，在启动后暂停音乐播放", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    }
                }) : ModLaunch._Closure$__.$I62-0, false);
            }
            else if (Settings.Get<bool>("UiMusicStart", null))
            {
                ModBase.RunInUi((ModLaunch._Closure$__.$I62-1 == null) ? (ModLaunch._Closure$__.$I62-1 = delegate
                {
                    if (ModMusic.MusicResume())
                    {
                        Logger.Info("已根据设置，在启动后开始音乐播放", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModLaunch.vb");
                    }
                }) : ModLaunch._Closure$__.$I62-1, false);
            }
            ModLaunch.McLaunchLog("启动器可见性：" + Conversions.ToString(Settings.Get<int>("LaunchArgumentVisible", null)), LogLevel.Info);
            switch (Settings.Get<int>("LaunchArgumentVisible", null))
            {
            case 0:
                ModLaunch.McLaunchLog("已根据设置，在启动后关闭启动器", LogLevel.Info);
                ModBase.RunInUi((ModLaunch._Closure$__.$I62-2 == null) ? (ModLaunch._Closure$__.$I62-2 = delegate
                {
                    ModMain.FrmMain.EndProgram(false);
                }) : ModLaunch._Closure$__.$I62-2, false);
                break;
            case 2:
            case 3:
                ModLaunch.McLaunchLog("已根据设置，在启动后隐藏启动器", LogLevel.Info);
                ModBase.RunInUi((ModLaunch._Closure$__.$I62-3 == null) ? (ModLaunch._Closure$__.$I62-3 = delegate
                {
                    ModMain.FrmMain.Hidden = true;
                }) : ModLaunch._Closure$__.$I62-3, false);
                break;
            case 4:
                ModLaunch.McLaunchLog("已根据设置，在启动后最小化启动器", LogLevel.Info);
                ModBase.RunInUi((ModLaunch._Closure$__.$I62-4 == null) ? (ModLaunch._Closure$__.$I62-4 = delegate
                {
                    ModMain.FrmMain.WindowState = WindowState.Minimized;
                }) : ModLaunch._Closure$__.$I62-4, false);
                break;
            }
            Settings.Set("SystemLaunchCount", checked(Settings.Get<int>("SystemLaunchCount", null) + 1), null);
        }

        public static ModLaunch.McLaunchOptions CurrentLaunchOptions = null;

        public static ModLoader.LoaderTask<ModLaunch.McLaunchOptions, object> McLaunchLoader = new ModLoader.LoaderTask<ModLaunch.McLaunchOptions, object>("Loader Launch", new Action<ModLoader.LoaderTask<ModLaunch.McLaunchOptions, object>>(ModLaunch.McLaunchStart), null, ThreadPriority.Normal)
        {
            OnStateChanged = delegate(ModLoader.LoaderBase a0)
            {
                ModLaunch.McLaunchState((ModLoader.LoaderTask<ModLaunch.McLaunchOptions, object>)a0);
            }
        };

        public static ModLoader.LoaderCombo<object> McLaunchLoaderReal;

        public static Process McLaunchProcess;

        public static ModWatcher.Watcher McLaunchWatcher;

        private static string CanceledHint = null;

        public static ModLoader.LoaderTask<ModLaunch.McLoginData, ModLaunch.McLoginResult> McLoginLoader = new ModLoader.LoaderTask<ModLaunch.McLoginData, ModLaunch.McLoginResult>("登录", new Action<ModLoader.LoaderTask<ModLaunch.McLoginData, ModLaunch.McLoginResult>>(ModLaunch.McLoginStart), new Func<ModLaunch.McLoginData>(ModLaunch.McLoginInput), ThreadPriority.BelowNormal)
        {
            ReloadTimeout = 1,
            ProgressWeight = 15.0,
            Block = false
        };

        public static ModLoader.LoaderTask<ModLaunch.McLoginMs, ModLaunch.McLoginResult> McLoginMsLoader = new ModLoader.LoaderTask<ModLaunch.McLoginMs, ModLaunch.McLoginResult>("Loader Login Ms", new Action<ModLoader.LoaderTask<ModLaunch.McLoginMs, ModLaunch.McLoginResult>>(ModLaunch.McLoginMsStart), null, ThreadPriority.Normal)
        {
            ReloadTimeout = 1
        };

        public static ModLoader.LoaderTask<ModLaunch.McLoginLegacy, ModLaunch.McLoginResult> McLoginLegacyLoader = new ModLoader.LoaderTask<ModLaunch.McLoginLegacy, ModLaunch.McLoginResult>("Loader Login Legacy", new Action<ModLoader.LoaderTask<ModLaunch.McLoginLegacy, ModLaunch.McLoginResult>>(ModLaunch.McLoginLegacyStart), null, ThreadPriority.Normal);

        public static ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult> McLoginNideLoader = new ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult>("Loader Login Nide", new Action<ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult>>(ModLaunch.McLoginServerStart), null, ThreadPriority.Normal)
        {
            ReloadTimeout = 600000
        };

        public static ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult> McLoginAuthLoader = new ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult>("Loader Login Auth", new Action<ModLoader.LoaderTask<ModLaunch.McLoginServer, ModLaunch.McLoginResult>>(ModLaunch.McLoginServerStart), null, ThreadPriority.Normal)
        {
            ReloadTimeout = 600000
        };

        public static Java McLaunchJavaSelected = null;

        private static string McLaunchArgument;

        private static Encoding McLaunchEncoding;

        public class McLaunchOptions
        {
            public McLaunchOptions()
            {
                this.ServerIp = null;
                this.SaveBatch = null;
                this.Instance = null;
                this.ExtraGameArgs = new List<string>();
            }

            public string ServerIp;

            public string SaveBatch;

            public ModMinecraft.McInstance Instance;

            public List<string> ExtraGameArgs;
        }

        public enum McLoginType
        {
            Legacy,
            Nide = 2,
            Auth,
            Ms = 5
        }

        public abstract class McLoginData
        {
            public override bool Equals(object obj)
            {
                return obj != null && obj.GetHashCode() == this.GetHashCode();
            }

            public ModLaunch.McLoginType Type;
        }

        public class McLoginServer : ModLaunch.McLoginData
        {
            public McLoginServer(ModLaunch.McLoginType Type)
            {
                this.ForceReselectProfile = false;
                this.Type = Type;
            }

            public override int GetHashCode()
            {
                return Convert.ToInt32(decimal.Remainder(new decimal(string.Concat(new string[]
                {
                    this.UserName,
                    this.Password,
                    this.BaseUrl,
                    this.Token,
                    Conversions.ToString((int)this.Type)
                }).GetStableHashCode()), 2147483647m));
            }

            public string UserName;

            public string Password;

            public string BaseUrl;

            public string Token;

            public string Description;

            public bool ForceReselectProfile;
        }

        public class McLoginMs : ModLaunch.McLoginData
        {
            public McLoginMs()
            {
                this.OAuthRefreshToken = "";
                this.AccessToken = "";
                this.Uuid = "";
                this.UserName = "";
                this.ProfileJson = "";
                this.Type = ModLaunch.McLoginType.Ms;
            }

            public override int GetHashCode()
            {
                return Convert.ToInt32(decimal.Remainder(new decimal((this.OAuthRefreshToken + this.UserName).GetStableHashCode()), 2147483647m));
            }

            public string OAuthRefreshToken;

            public string AccessToken;

            public string Uuid;

            public string UserName;

            public string ProfileJson;
        }

        public class McLoginLegacy : ModLaunch.McLoginData
        {
            public McLoginLegacy()
            {
                this.Type = ModLaunch.McLoginType.Legacy;
            }

            public override int GetHashCode()
            {
                return Convert.ToInt32(decimal.Remainder(new decimal((this.UserName + Conversions.ToString(this.SkinType) + this.SkinName + Conversions.ToString((int)this.Type)).GetStableHashCode()), 2147483647m));
            }

            public string UserName;

            public int SkinType;

            public string SkinName;
        }

        public struct McLoginResult
        {
            public string Name;

            public string Uuid;

            public string AccessToken;

            public string Type;

            public string ClientToken;

            public string ProfileJson;
        }
    }
}
