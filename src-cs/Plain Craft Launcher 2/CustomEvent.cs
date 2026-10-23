using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class CustomEvent : DependencyObject
    {
        public CustomEvent.EventType Type
        {
            get
            {
                CustomEvent.EventType Value = CustomEvent.EventType.None;
                ModBase.RunInUiWait(delegate
                {
                    Value = (CustomEvent.EventType)Conversions.ToInteger(this.GetValue(CustomEvent.TypeProperty));
                });
                return Value;
            }
            set
            {
                base.SetValue(CustomEvent.TypeProperty, value);
            }
        }

        public string Data
        {
            get
            {
                string Value = null;
                ModBase.RunInUiWait(delegate
                {
                    Value = Conversions.ToString(this.GetValue(CustomEvent.DataProperty));
                });
                return Value;
            }
            set
            {
                base.SetValue(CustomEvent.DataProperty, value);
            }
        }

        public CustomEvent()
        {
        }

        public CustomEvent(CustomEvent.EventType Type, string Data)
        {
            this.Type = Type;
            this.Data = Data;
        }

        public void Raise()
        {
            CustomEvent.Raise(this.Type, this.Data);
        }

        public static void Raise(CustomEvent.EventType Type, string Arg)
        {
            if (Type != CustomEvent.EventType.None)
            {
                Logger.Info(string.Format("执行自定义事件：{0}, {1}", Type, Arg), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                if (Arg == null)
                {
                    Arg = "";
                }
                string[] Args = Arg.Split("|", false);
                string Arg0 = Arg.BeforeFirst("|", false);
                string Arg1 = Arg.AfterFirst("|", false);
                try
                {
                    switch (Type)
                    {
                    case CustomEvent.EventType.打开网页:
                        Arg = Arg.Replace("\\", "/");
                        if (!Arg.Contains("://") || Arg.StartsWithF("file", true))
                        {
                            ModMain.MyMsgBox("EventData 必须为一个网址。\r\n如果想要启动程序，请将 EventType 改为 打开文件。", "事件执行失败", "确定", "", "", false, true, false, null, null, null);
                            return;
                        }
                        ModMain.Hint("正在开启，请稍候：" + Arg, ModMain.HintType.Blue, true);
                        ModBase.RunInThread(delegate
                        {
                            ModBase.OpenWebsite(Arg);
                        });
                        goto IL_0683;
                    case CustomEvent.EventType.打开文件:
                    case CustomEvent.EventType.打开帮助:
                    case CustomEvent.EventType.执行命令:
                        ModBase.RunInThread(delegate
                        {
                            try
                            {
                                string[] absoluteUrls = CustomEvent.GetAbsoluteUrls(Arg0, Type);
                                string text = absoluteUrls[0];
                                string text2 = absoluteUrls[1];
                                Logger.Info(string.Format("打开类自定义事件实际路径：{0}，工作目录：{1}", text, text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                                if (Type == CustomEvent.EventType.打开帮助)
                                {
                                    PageOtherHelp.EnterHelpPage(text);
                                }
                                else if (CustomEvent.EventSafetyConfirm("即将执行：" + text + ((Args.Length >= 2) ? (" " + Arg1) : "")))
                                {
                                    ModBase.StartProcess(new ProcessStartInfo
                                    {
                                        Arguments = ((Args.Length >= 2) ? Arg1 : ""),
                                        FileName = text,
                                        WorkingDirectory = text2
                                    });
                                }
                            }
                            catch (Exception ex3)
                            {
                                Logger.Error(ex3, "执行打开类自定义事件失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                            }
                        });
                        goto IL_0683;
                    case CustomEvent.EventType.启动游戏:
                        if (Operators.CompareString(Arg0, "\\current", false) == 0)
                        {
                            if (ModMinecraft.McInstanceSelected == null)
                            {
                                ModMain.Hint("请先选择一个 Minecraft 版本！", ModMain.HintType.Red, true);
                                return;
                            }
                            Arg0 = ModMinecraft.McInstanceSelected.Name;
                        }
                        ModBase.RunInUi(delegate
                        {
                            if (ModLaunch.McLaunchStart(new ModLaunch.McLaunchOptions
                            {
                                ServerIp = ((Args.Length >= 2) ? Args[1] : null),
                                Instance = new ModMinecraft.McInstance(Arg0)
                            }))
                            {
                                ModMain.Hint(string.Format("正在启动 {0}……", Arg0), ModMain.HintType.Blue, true);
                            }
                        }, false);
                        goto IL_0683;
                    case CustomEvent.EventType.复制文本:
                        ModBase.ClipboardSet(Arg, true);
                        goto IL_0683;
                    case CustomEvent.EventType.刷新主页:
                        ModBase.RunInUi(delegate
                        {
                            ModMain.FrmLaunchRight.ForceRefresh();
                            if (string.IsNullOrEmpty(Arg))
                            {
                                ModMain.Hint("已刷新！", ModMain.HintType.Green, true);
                            }
                        }, false);
                        goto IL_0683;
                    case CustomEvent.EventType.刷新页面:
                        if (!(ModMain.FrmMain.PageRight is IRefreshable))
                        {
                            ModMain.Hint("当前页面不支持刷新操作！", ModMain.HintType.Red, true);
                            goto IL_0683;
                        }
                        ModBase.RunInUiWait((CustomEvent._Closure$__.$I13-4 == null) ? (CustomEvent._Closure$__.$I13-4 = delegate
                        {
                            ((IRefreshable)ModMain.FrmMain.PageRight).Refresh();
                        }) : CustomEvent._Closure$__.$I13-4);
                        if (string.IsNullOrEmpty(Arg))
                        {
                            ModMain.Hint("已刷新！", ModMain.HintType.Green, true);
                            goto IL_0683;
                        }
                        goto IL_0683;
                    case CustomEvent.EventType.刷新帮助:
                        ModBase.RunInUiWait((CustomEvent._Closure$__.$I13-5 == null) ? (CustomEvent._Closure$__.$I13-5 = delegate
                        {
                            PageOtherLeft.RefreshHelp();
                        }) : CustomEvent._Closure$__.$I13-5);
                        if (string.IsNullOrEmpty(Arg))
                        {
                            ModMain.Hint("已刷新！", ModMain.HintType.Green, true);
                            goto IL_0683;
                        }
                        goto IL_0683;
                    case CustomEvent.EventType.今日人品:
                        PageOtherTest.Jrrp();
                        goto IL_0683;
                    case CustomEvent.EventType.内存优化:
                        ModBase.RunInThread((CustomEvent._Closure$__.$I13-6 == null) ? (CustomEvent._Closure$__.$I13-6 = delegate
                        {
                            PageOtherTest.MemoryOptimize(true);
                        }) : CustomEvent._Closure$__.$I13-6);
                        goto IL_0683;
                    case CustomEvent.EventType.清理垃圾:
                        ModBase.RunInThread((CustomEvent._Closure$__.$I13-7 == null) ? (CustomEvent._Closure$__.$I13-7 = delegate
                        {
                            PageOtherTest.RubbishClear();
                        }) : CustomEvent._Closure$__.$I13-7);
                        goto IL_0683;
                    case CustomEvent.EventType.弹出窗口:
                        if (Args.Length == 1)
                        {
                            throw new Exception(string.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 弹窗标题|弹窗内容", Type));
                        }
                        ModMain.MyMsgBox(Arg1.BeforeLast("|", false).Replace("\\n", "\r\n"), Arg0.Replace("\\n", "\r\n"), (Args.Length > 2) ? Arg1.AfterLast("|", false) : "确定", "", "", false, true, false, null, null, null);
                        goto IL_0683;
                    case CustomEvent.EventType.弹出提示:
                        ModMain.Hint(Arg0.Replace("\\n", "\r\n"), (Args.Length == 1) ? ModMain.HintType.Blue : EnumUtils.FromString<ModMain.HintType>(Arg1), true);
                        goto IL_0683;
                    case CustomEvent.EventType.切换页面:
                        ModBase.RunInUi(delegate
                        {
                            FormMain.PageType pageType = EnumUtils.FromString<FormMain.PageType>(Arg0);
                            if (pageType == FormMain.PageType.InstanceSetup)
                            {
                                if (ModMinecraft.McInstanceSelected == null)
                                {
                                    ModMain.Hint("请先选择一个 Minecraft 版本！", ModMain.HintType.Red, true);
                                    return;
                                }
                                if (ModMinecraft.McInstanceLock.IsLocked(ModMinecraft.McInstanceSelected))
                                {
                                    ModMain.Hint("该版本正在被修改，暂时无法进入版本设置！", ModMain.HintType.Red, true);
                                    return;
                                }
                                PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected;
                            }
                            ModMain.FrmMain.PageChange(pageType, (Args.Length == 1) ? FormMain.PageSubType.Default : EnumUtils.FromString<FormMain.PageSubType>(Arg1));
                        }, false);
                        goto IL_0683;
                    case CustomEvent.EventType.导入整合包:
                    case CustomEvent.EventType.安装整合包:
                        ModBase.RunInUi((CustomEvent._Closure$__.$I13-9 == null) ? (CustomEvent._Closure$__.$I13-9 = delegate
                        {
                            ModModpack.ModpackInstall();
                        }) : CustomEvent._Closure$__.$I13-9, false);
                        goto IL_0683;
                    case CustomEvent.EventType.下载文件:
                        Arg0 = Arg0.Replace("\\", "/");
                        if (!Arg0.StartsWithF("http://", true) && !Arg0.StartsWithF("https://", true))
                        {
                            ModMain.MyMsgBox("EventData 必须为以 http:// 或 https:// 开头的网址。\r\nPCL 不支持其他乱七八糟的下载协议。", "事件执行失败", "确定", "", "", false, true, false, null, null, null);
                            return;
                        }
                        if (!CustomEvent.EventSafetyConfirm("即将从该网址下载文件：\r\n" + Arg0))
                        {
                            return;
                        }
                        ModBase.RunInUi(delegate
                        {
                            try
                            {
                                int num = Args.Length;
                                if (num != 1)
                                {
                                    if (num != 2)
                                    {
                                        PageOtherTest.StartCustomDownload(Arg0, Args[1], Args[2]);
                                    }
                                    else
                                    {
                                        PageOtherTest.StartCustomDownload(Arg0, Arg1, null);
                                    }
                                }
                                else
                                {
                                    PageOtherTest.StartCustomDownload(Arg0, PathUtils.GetLastPart(Arg0), null);
                                }
                            }
                            catch (Exception ex4)
                            {
                                PageOtherTest.StartCustomDownload(Arg0, "未知", null);
                            }
                        }, false);
                        goto IL_0683;
                    case CustomEvent.EventType.修改设置:
                    case CustomEvent.EventType.写入设置:
                        if (Args.Length == 1)
                        {
                            throw new Exception(string.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 UiLauncherTransparent|400", Type));
                        }
                        Settings.SetSafe(Arg0, Args[1], ModMinecraft.McInstanceSelected);
                        if (Args.Length == 2)
                        {
                            ModMain.Hint(string.Format("已写入设置：{0} → {1}", Arg0, Args[1]), ModMain.HintType.Green, true);
                            goto IL_0683;
                        }
                        goto IL_0683;
                    case CustomEvent.EventType.修改变量:
                    case CustomEvent.EventType.写入变量:
                        if (Args.Length == 1)
                        {
                            throw new Exception(string.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 VariableName|SomeValue", Type));
                        }
                        try
                        {
                            RegistryUtils.Write(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), string.Format("CustomEvent{0}", Arg0), Args[1]);
                            if (Args.Length == 2)
                            {
                                ModMain.Hint(string.Format("已写入变量：{0} → {1}", Arg0, Args[1]), ModMain.HintType.Green, true);
                            }
                            goto IL_0683;
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, string.Format("写入变量失败：{0} → {1}", Arg0, Args[1]), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                            goto IL_0683;
                        }
                        break;
                    case CustomEvent.EventType.加入房间:
                        break;
                    case CustomEvent.EventType.检查更新:
                        ModSecret.UpdateCheckByButton();
                        goto IL_0683;
                    default:
                        ModMain.MyMsgBox("未知的事件类型：" + Conversions.ToString((int)Type) + "\r\n请检查事件类型填写是否正确，或者 PCL 是否为最新版本。", "事件执行失败", "确定", "", "", false, true, false, null, null, null);
                        goto IL_0683;
                    }
                    ModBase.RunInUi(delegate
                    {
                        if (string.IsNullOrWhiteSpace(Arg))
                        {
                            PageLinkMain.Join();
                            return;
                        }
                        if (CustomEvent.EventSafetyConfirm("即将加入第三方联机房间。"))
                        {
                            PageLinkMain.Join(Arg);
                        }
                    }, false);
                    IL_0683:;
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, string.Format("事件执行失败（{0}, {1}）", Type, Arg), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                }
            }
        }

        public static string[] GetAbsoluteUrls(string RelativeUrl, CustomEvent.EventType Type)
        {
            if (RelativeUrl.StartsWithF("http", true))
            {
                if (ModBase.RunInUi())
                {
                    throw new Exception("能打开联网帮助页面的 MyListItem 必须手动设置 Title、Info 属性！");
                }
                string lastPart;
                try
                {
                    lastPart = PathUtils.GetLastPart(RelativeUrl);
                    if (Operators.CompareString(PathUtils.GetExtension(lastPart), "json", false) != 0)
                    {
                        throw new Exception("未指向 .json 后缀的文件");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("联网帮助页面须指向一个帮助 JSON 文件，并在同路径下包含相应 XAML 文件！\r\n例如：\r\n - https://www.baidu.com/test.json（填写这个路径）\r\n - https://www.baidu.com/test.xaml（同时也需要包含这个文件）", ex);
                }
                string text = ModMain.RequestTaskTempFolder(false) + lastPart;
                Logger.Info(string.Format("转换网络资源：{0} -> {1}", RelativeUrl, text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                try
                {
                    ModNet.NetDownloadByClient(RelativeUrl, text, false);
                    ModNet.NetDownloadByClient(RelativeUrl.Replace(".json", ".xaml"), text.Replace(".json", ".xaml"), false);
                }
                catch (Exception ex2)
                {
                    throw new Exception("下载指定的文件失败！\r\n注意，联网帮助页面须指向一个帮助 JSON 文件，并在同路径下包含相应 XAML 文件！\r\n例如：\r\n - https://www.baidu.com/test.json（填写这个路径）\r\n - https://www.baidu.com/test.xaml（同时也需要包含这个文件）", ex2);
                }
                RelativeUrl = text;
            }
            RelativeUrl = RelativeUrl.Replace("/", "\\").Lower().TrimStart(new char[] { '\\' });
            string text2 = Paths.Base + "PCL";
            ModMain.HelpTryExtract();
            string text3;
            if (RelativeUrl.Contains(":\\"))
            {
                text3 = RelativeUrl;
                Logger.Info(string.Format("自定义事件中由绝对路径{0}：{1}", Type, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
            }
            else if (FileUtils.Exists(Paths.Base + "PCL\\" + RelativeUrl))
            {
                text3 = Paths.Base + "PCL\\" + RelativeUrl;
                Logger.Info(string.Format("自定义事件中由相对 PCL 文件夹的路径{0}：{1}", Type, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
            }
            else if (FileUtils.Exists(Paths.Base + "PCL\\Help\\" + RelativeUrl))
            {
                text3 = Paths.Base + "PCL\\Help\\" + RelativeUrl;
                text2 = Paths.Base + "PCL\\Help\\";
                Logger.Info(string.Format("自定义事件中由相对 PCL 本地帮助文件夹的路径{0}：{1}", Type, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
            }
            else if (Type == CustomEvent.EventType.打开帮助 && FileUtils.Exists(ModBase.PathTemp + "Help\\" + RelativeUrl))
            {
                text3 = ModBase.PathTemp + "Help\\" + RelativeUrl;
                text2 = ModBase.PathTemp + "Help\\";
                Logger.Info(string.Format("自定义事件中由相对 PCL 自带帮助文件夹的路径{0}：{1}", Type, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
            }
            else
            {
                if (Type != CustomEvent.EventType.打开文件 && Type != CustomEvent.EventType.执行命令)
                {
                    throw new FileNotFoundException("未找到 EventData 指向的本地 xaml 文件：" + RelativeUrl, RelativeUrl);
                }
                text3 = RelativeUrl;
                Logger.Info(string.Format("自定义事件中直接{0}：{1}", Type, text3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
            }
            return new string[] { text3, text2 };
        }

        private static bool EventSafetyConfirm(string Message)
        {
            bool flag;
            if (Settings.Get<bool>("HintCustomCommand", null))
            {
                flag = true;
            }
            else
            {
                int num = ModMain.MyMsgBox(Message + "\r\n请在确认没有安全隐患后再继续。", "执行确认", "继续", "继续且今后不再要求确认", "取消", false, true, false, null, null, null);
                if (num != 1)
                {
                    if (num != 2)
                    {
                        flag = false;
                    }
                    else
                    {
                        Settings.Set("HintCustomCommand", true, null);
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
            return flag;
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(CustomEvent.EventType), typeof(CustomEvent), new PropertyMetadata(CustomEvent.EventType.None));

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(string), typeof(CustomEvent), new PropertyMetadata(null));

        public enum EventType
        {
            None,
            打开网页,
            打开文件,
            打开帮助,
            执行命令,
            启动游戏,
            复制文本,
            刷新主页,
            刷新页面,
            刷新帮助,
            今日人品,
            内存优化,
            清理垃圾,
            弹出窗口,
            弹出提示,
            切换页面,
            导入整合包,
            安装整合包,
            下载文件,
            修改设置,
            写入设置,
            修改变量,
            写入变量,
            加入房间,
            检查更新
        }
    }
}
