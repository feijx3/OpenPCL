using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class CustomEvent : DependencyObject
    {
        // Note: this type is marked as 'beforefieldinit'.
        static CustomEvent()
        {
        }

        public CustomEvent.EventType Type
        {
            get
            {
                CustomEvent._Closure___2_0 CS_locals1 = new CustomEvent._Closure___2_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_Value = CustomEvent.EventType.None;
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
                return CS_locals1._VB_Local_Value;
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
                CustomEvent._Closure___6_0 CS_locals1 = new CustomEvent._Closure___6_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_Value = null;
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
                return CS_locals1._VB_Local_Value;
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
            CustomEvent._Closure___13_0 CS_locals1 = new CustomEvent._Closure___13_0();
            CS_locals1._VB_Local_Type = Type;
            CS_locals1._VB_Local_Arg = Arg;
            if (CS_locals1._VB_Local_Type != CustomEvent.EventType.None)
            {
                Logger.Info(string.Format("执行自定义事件：{0}, {1}", CS_locals1._VB_Local_Type, CS_locals1._VB_Local_Arg), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                if (CS_locals1._VB_Local_Arg == null)
                {
                    CS_locals1._VB_Local_Arg = "";
                }
                CS_locals1._VB_Local_Args = CS_locals1._VB_Local_Arg.Split("|", false);
                CS_locals1._VB_Local_Arg0 = CS_locals1._VB_Local_Arg.BeforeFirst("|", false);
                CS_locals1._VB_Local_Arg1 = CS_locals1._VB_Local_Arg.AfterFirst("|", false);
                try
                {
                    switch (CS_locals1._VB_Local_Type)
                    {
                    case CustomEvent.EventType.打开网页:
                        CS_locals1._VB_Local_Arg = CS_locals1._VB_Local_Arg.Replace("\\", "/");
                        if (!CS_locals1._VB_Local_Arg.Contains("://") || CS_locals1._VB_Local_Arg.StartsWithF("file", true))
                        {
                            ModMain.OurMsgBox("EventData 必须为一个网址。\r\n如果想要启动程序，请将 EventType 改为 打开文件。", "事件执行失败", "确定", "", "", false, true, false, null, null, null);
                            return;
                        }
                        ModMain.Hint("正在开启，请稍候：" + CS_locals1._VB_Local_Arg, ModMain.HintType.Blue, true);
                        ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
                        goto IL_0683;
                    case CustomEvent.EventType.打开文件:
                    case CustomEvent.EventType.打开帮助:
                    case CustomEvent.EventType.执行命令:
                        ModBase.RunInThread(new Action(CS_locals1._Lambda___1));
                        goto IL_0683;
                    case CustomEvent.EventType.启动游戏:
                        if (Operators.CompareString(CS_locals1._VB_Local_Arg0, "\\current", false) == 0)
                        {
                            if (((object)(ModMinecraft.McInstanceSelected) == (object)null))
                            {
                                ModMain.Hint("请先选择一个 Minecraft 版本！", ModMain.HintType.Red, true);
                                return;
                            }
                            CS_locals1._VB_Local_Arg0 = ModMinecraft.McInstanceSelected.Name;
                        }
                        ModBase.RunInUi(new Action(CS_locals1._Lambda___2), false);
                        goto IL_0683;
                    case CustomEvent.EventType.复制文本:
                        ModBase.ClipboardSet(CS_locals1._VB_Local_Arg, true);
                        goto IL_0683;
                    case CustomEvent.EventType.刷新主页:
                        ModBase.RunInUi(new Action(CS_locals1._Lambda___3), false);
                        goto IL_0683;
                    case CustomEvent.EventType.刷新页面:
                        if (!(ModMain.FrmMain.PageRight is IRefreshable))
                        {
                            ModMain.Hint("当前页面不支持刷新操作！", ModMain.HintType.Red, true);
                            goto IL_0683;
                        }
                        ModBase.RunInUiWait((CustomEvent._Closure___._I13_4 == null) ? (CustomEvent._Closure___._I13_4 = new Action(CustomEvent._Closure___._I._Lambda___13_4)) : CustomEvent._Closure___._I13_4);
                        if (string.IsNullOrEmpty(CS_locals1._VB_Local_Arg))
                        {
                            ModMain.Hint("已刷新！", ModMain.HintType.Green, true);
                            goto IL_0683;
                        }
                        goto IL_0683;
                    case CustomEvent.EventType.刷新帮助:
                        ModBase.RunInUiWait((CustomEvent._Closure___._I13_5 == null) ? (CustomEvent._Closure___._I13_5 = new Action(CustomEvent._Closure___._I._Lambda___13_5)) : CustomEvent._Closure___._I13_5);
                        if (string.IsNullOrEmpty(CS_locals1._VB_Local_Arg))
                        {
                            ModMain.Hint("已刷新！", ModMain.HintType.Green, true);
                            goto IL_0683;
                        }
                        goto IL_0683;
                    case CustomEvent.EventType.今日人品:
                        PageOtherTest.Jrrp();
                        goto IL_0683;
                    case CustomEvent.EventType.内存优化:
                        ModBase.RunInThread((CustomEvent._Closure___._I13_6 == null) ? (CustomEvent._Closure___._I13_6 = new Action(CustomEvent._Closure___._I._Lambda___13_6)) : CustomEvent._Closure___._I13_6);
                        goto IL_0683;
                    case CustomEvent.EventType.清理垃圾:
                        ModBase.RunInThread((CustomEvent._Closure___._I13_7 == null) ? (CustomEvent._Closure___._I13_7 = new Action(CustomEvent._Closure___._I._Lambda___13_7)) : CustomEvent._Closure___._I13_7);
                        goto IL_0683;
                    case CustomEvent.EventType.弹出窗口:
                        if (CS_locals1._VB_Local_Args.Length == 1)
                        {
                            throw new Exception(string.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 弹窗标题|弹窗内容", CS_locals1._VB_Local_Type));
                        }
                        ModMain.OurMsgBox(CS_locals1._VB_Local_Arg1.BeforeLast("|", false).Replace("\\n", "\r\n"), CS_locals1._VB_Local_Arg0.Replace("\\n", "\r\n"), (CS_locals1._VB_Local_Args.Length > 2) ? CS_locals1._VB_Local_Arg1.AfterLast("|", false) : "确定", "", "", false, true, false, null, null, null);
                        goto IL_0683;
                    case CustomEvent.EventType.弹出提示:
                        ModMain.Hint(CS_locals1._VB_Local_Arg0.Replace("\\n", "\r\n"), (CS_locals1._VB_Local_Args.Length == 1) ? ModMain.HintType.Blue : EnumUtils.FromString<ModMain.HintType>(CS_locals1._VB_Local_Arg1), true);
                        goto IL_0683;
                    case CustomEvent.EventType.切换页面:
                        ModBase.RunInUi(new Action(CS_locals1._Lambda___8), false);
                        goto IL_0683;
                    case CustomEvent.EventType.导入整合包:
                    case CustomEvent.EventType.安装整合包:
                        ModBase.RunInUi((CustomEvent._Closure___._I13_9 == null) ? (CustomEvent._Closure___._I13_9 = new Action(CustomEvent._Closure___._I._Lambda___13_9)) : CustomEvent._Closure___._I13_9, false);
                        goto IL_0683;
                    case CustomEvent.EventType.下载文件:
                        CS_locals1._VB_Local_Arg0 = CS_locals1._VB_Local_Arg0.Replace("\\", "/");
                        if (!CS_locals1._VB_Local_Arg0.StartsWithF("http://", true) && !CS_locals1._VB_Local_Arg0.StartsWithF("https://", true))
                        {
                            ModMain.OurMsgBox("EventData 必须为以 http:// 或 https:// 开头的网址。\r\nPCL 不支持其他乱七八糟的下载协议。", "事件执行失败", "确定", "", "", false, true, false, null, null, null);
                            return;
                        }
                        if (!CustomEvent.EventSafetyConfirm("即将从该网址下载文件：\r\n" + CS_locals1._VB_Local_Arg0))
                        {
                            return;
                        }
                        ModBase.RunInUi(new Action(CS_locals1._Lambda___10), false);
                        goto IL_0683;
                    case CustomEvent.EventType.修改设置:
                    case CustomEvent.EventType.写入设置:
                        if (CS_locals1._VB_Local_Args.Length == 1)
                        {
                            throw new Exception(string.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 UiLauncherTransparent|400", CS_locals1._VB_Local_Type));
                        }
                        Settings.SetSafe(CS_locals1._VB_Local_Arg0, CS_locals1._VB_Local_Args[1], ModMinecraft.McInstanceSelected);
                        if (CS_locals1._VB_Local_Args.Length == 2)
                        {
                            ModMain.Hint(string.Format("已写入设置：{0} → {1}", CS_locals1._VB_Local_Arg0, CS_locals1._VB_Local_Args[1]), ModMain.HintType.Green, true);
                            goto IL_0683;
                        }
                        goto IL_0683;
                    case CustomEvent.EventType.修改变量:
                    case CustomEvent.EventType.写入变量:
                        if (CS_locals1._VB_Local_Args.Length == 1)
                        {
                            throw new Exception(string.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 VariableName|SomeValue", CS_locals1._VB_Local_Type));
                        }
                        try
                        {
                            RegistryUtils.Write(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), string.Format("CustomEvent{0}", CS_locals1._VB_Local_Arg0), CS_locals1._VB_Local_Args[1]);
                            if (CS_locals1._VB_Local_Args.Length == 2)
                            {
                                ModMain.Hint(string.Format("已写入变量：{0} → {1}", CS_locals1._VB_Local_Arg0, CS_locals1._VB_Local_Args[1]), ModMain.HintType.Green, true);
                            }
                            goto IL_0683;
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, string.Format("写入变量失败：{0} → {1}", CS_locals1._VB_Local_Arg0, CS_locals1._VB_Local_Args[1]), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                            goto IL_0683;
                        }
                        break;
                    case CustomEvent.EventType.加入房间:
                        break;
                    case CustomEvent.EventType.检查更新:
                        ModSecret.UpdateCheckByButton();
                        goto IL_0683;
                    default:
                        ModMain.OurMsgBox("未知的事件类型：" + Conversions.ToString((int)CS_locals1._VB_Local_Type) + "\r\n请检查事件类型填写是否正确，或者 PCL 是否为最新版本。", "事件执行失败", "确定", "", "", false, true, false, null, null, null);
                        goto IL_0683;
                    }
                    ModBase.RunInUi(new Action(CS_locals1._Lambda___11), false);
                    IL_0683:;
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, string.Format("事件执行失败（{0}, {1}）", CS_locals1._VB_Local_Type, CS_locals1._VB_Local_Arg), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                }
            }
        }

        public static string[] GetAbsoluteUrls(string RelativeUrl, CustomEvent.EventType Type)
        {
            if (RelativeUrl.StartsWithF("http", true))
            {
                if (ModBase.RunInUi())
                {
                    throw new Exception("能打开联网帮助页面的 OurListItem 必须手动设置 Title、Info 属性！");
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
                int num = ModMain.OurMsgBox(Message + "\r\n请在确认没有安全隐患后再继续。", "执行确认", "继续", "继续且今后不再要求确认", "取消", false, true, false, null, null, null);
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

            internal void _Lambda___13_4()
            {
                ((IRefreshable)ModMain.FrmMain.PageRight).Refresh();
            }

            internal void _Lambda___13_5()
            {
                PageOtherLeft.RefreshHelp();
            }

            internal void _Lambda___13_6()
            {
                PageOtherTest.MemoryOptimize(true);
            }

            internal void _Lambda___13_7()
            {
                PageOtherTest.RubbishClear();
            }

            internal void _Lambda___13_9()
            {
                ModModpack.ModpackInstall();
            }

            public static readonly CustomEvent._Closure___ _I = new CustomEvent._Closure___();

            public static Action _I13_4;

            public static Action _I13_5;

            public static Action _I13_6;

            public static Action _I13_7;

            public static Action _I13_9;
        }

        [CompilerGenerated]
        private sealed class _Closure___13_0
        {
            public _Closure___13_0()
            {
            }

            internal void _Lambda___0()
            {
                ModBase.OpenWebsite(this._VB_Local_Arg);
            }

            internal void _Lambda___1()
            {
                try
                {
                    string[] absoluteUrls = CustomEvent.GetAbsoluteUrls(this._VB_Local_Arg0, this._VB_Local_Type);
                    string text = absoluteUrls[0];
                    string text2 = absoluteUrls[1];
                    Logger.Info(string.Format("打开类自定义事件实际路径：{0}，工作目录：{1}", text, text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                    if (this._VB_Local_Type == CustomEvent.EventType.打开帮助)
                    {
                        PageOtherHelp.EnterHelpPage(text);
                    }
                    else if (CustomEvent.EventSafetyConfirm("即将执行：" + text + ((this._VB_Local_Args.Length >= 2) ? (" " + this._VB_Local_Arg1) : "")))
                    {
                        ModBase.StartProcess(new ProcessStartInfo
                        {
                            Arguments = ((this._VB_Local_Args.Length >= 2) ? this._VB_Local_Arg1 : ""),
                            FileName = text,
                            WorkingDirectory = text2
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "执行打开类自定义事件失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModEvent.vb");
                }
            }

            internal void _Lambda___2()
            {
                if (ModLaunch.McLaunchStart(new ModLaunch.McLaunchOptions
                {
                    ServerIp = ((this._VB_Local_Args.Length >= 2) ? this._VB_Local_Args[1] : null),
                    Instance = new ModMinecraft.McInstance(this._VB_Local_Arg0)
                }))
                {
                    ModMain.Hint(string.Format("正在启动 {0}……", this._VB_Local_Arg0), ModMain.HintType.Blue, true);
                }
            }

            internal void _Lambda___3()
            {
                ModMain.FrmLaunchRight.ForceRefresh();
                if (string.IsNullOrEmpty(this._VB_Local_Arg))
                {
                    ModMain.Hint("已刷新！", ModMain.HintType.Green, true);
                }
            }

            internal void _Lambda___8()
            {
                FormMain.PageType pageType = EnumUtils.FromString<FormMain.PageType>(this._VB_Local_Arg0);
                if (pageType == FormMain.PageType.InstanceSetup)
                {
                    if (((object)(ModMinecraft.McInstanceSelected) == (object)null))
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
                ModMain.FrmMain.PageChange(pageType, (this._VB_Local_Args.Length == 1) ? FormMain.PageSubType.Default : EnumUtils.FromString<FormMain.PageSubType>(this._VB_Local_Arg1));
            }

            internal void _Lambda___10()
            {
                try
                {
                    int num = this._VB_Local_Args.Length;
                    if (num != 1)
                    {
                        if (num != 2)
                        {
                            PageOtherTest.StartCustomDownload(this._VB_Local_Arg0, this._VB_Local_Args[1], this._VB_Local_Args[2]);
                        }
                        else
                        {
                            PageOtherTest.StartCustomDownload(this._VB_Local_Arg0, this._VB_Local_Arg1, null);
                        }
                    }
                    else
                    {
                        PageOtherTest.StartCustomDownload(this._VB_Local_Arg0, PathUtils.GetLastPart(this._VB_Local_Arg0), null);
                    }
                }
                catch (Exception ex)
                {
                    PageOtherTest.StartCustomDownload(this._VB_Local_Arg0, "未知", null);
                }
            }

            internal void _Lambda___11()
            {
                if (string.IsNullOrWhiteSpace(this._VB_Local_Arg))
                {
                    PageLinkMain.Join();
                    return;
                }
                if (CustomEvent.EventSafetyConfirm("即将加入第三方联机房间。"))
                {
                    PageLinkMain.Join(this._VB_Local_Arg);
                }
            }

            public string _VB_Local_Arg;

            public string _VB_Local_Arg0;

            public CustomEvent.EventType _VB_Local_Type;

            public string[] _VB_Local_Args;

            public string _VB_Local_Arg1;
        }

        [CompilerGenerated]
        private sealed class _Closure___2_0
        {
            public _Closure___2_0()
            {
            }

            internal void _Lambda___0()
            {
                this._VB_Local_Value = (CustomEvent.EventType)Conversions.ToInteger(this._VB_Me.GetValue(CustomEvent.TypeProperty));
            }

            public CustomEvent.EventType _VB_Local_Value;

            public CustomEvent _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            public _Closure___6_0()
            {
            }

            internal void _Lambda___0()
            {
                this._VB_Local_Value = Conversions.ToString(this._VB_Me.GetValue(CustomEvent.DataProperty));
            }

            public string _VB_Local_Value;

            public CustomEvent _VB_Me;
        }
    }
}
